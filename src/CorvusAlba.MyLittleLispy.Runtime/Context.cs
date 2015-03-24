using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace CorvusAlba.MyLittleLispy.Runtime
{
    public class Context
    {
        private readonly Stack<Frame> _callStack = new Stack<Frame>();
        private readonly Dictionary<string, Func<Node[], Value>> _specialForms;
        private readonly Frame _globalFrame;
        private Parser _parser;
        private bool _evalMacro = true;

        public Frame CurrentFrame
        {
            get
            {
                return _callStack.Peek();
            }
        }

        private Value InvokeCondClause(Expression clause, Value condition = null)
        {
            var tail = clause.Tail.ToArray();
            var first = tail.First().Quote(this);
            if (first is SymbolValue && first.To<string>() == "=>")
            {
                if (condition != null)
                {
                    return new Closure(this, null, new Expression(tail.Skip(1).
                                          Concat(new[] { condition.ToExpression() })), true);
                }

                throw new SyntaxErrorException();
            }
            return new Closure(this, null, new Expression(new[] { new Symbol(new SymbolValue("begin")) }.
                                  Concat(tail).ToArray()), true);
        }

        public Context(Parser parser)
        {
            _parser = parser;
            _specialForms = new Dictionary<string, Func<Node[], Value>>
		    {
		        {"eval", args => Trampoline(args[0].Eval(this)).ToExpression().Eval(this) },
		        {"define", args => Define(args[0], new Expression(new [] { new Symbol(new SymbolValue("begin")) }.
								          Concat(args.Skip(1)).ToArray())) },
		        {"defmacro", args => DefineMacro(args[0].Quote(this).To<string>(), args[1], new Expression(new [] { new Symbol(new SymbolValue("begin")) }.
								          Concat(args.Skip(2)).ToArray())) },
		        {
		            "macroexpand", args =>
		            {
		                _evalMacro = false;
		                var result = Trampoline(args[0].Eval(this)).ToExpression().Eval(this);
		                _evalMacro = true;
		                return result;
		            }
		        },
		        {"quote", args => args[0].Quote(this) },
		        {"quasiquote", Quasiquote },
		        {"unquote", args => Trampoline(args[0].Eval(this)) },
		        {"unquote-splicing", args => Trampoline(args[0].Eval(this)) },
		        {"list", args => new Cons(args.Select(node => Trampoline(node.Eval(this))).ToArray()) },
		        {"cons", args => new Cons(Trampoline(args[0].Eval(this)), Trampoline(args[1].Eval(this))) },
		        {"lambda", args => new Closure(this, args[0],
						       new Expression(new [] { new Symbol(new SymbolValue("begin")) }.
								      Concat(args.Skip(1)).ToArray())) },
		        {"when", args => Trampoline(args[0].Eval(this)).To<bool>()
		             ? (Value) new Closure(this, null, new Expression(new [] { new Symbol(new SymbolValue("begin")) }.
								      Concat(args.Skip(1)).ToArray()), true)
		             : (Value) Null.Value },
		        {"unless", args => !Trampoline(args[0].Eval(this)).To<bool>()
		             ? (Value) new Closure(this, null, new Expression(new [] { new Symbol(new SymbolValue("begin")) }.
								      Concat(args.Skip(1)).ToArray()), true)
		             : (Value) Null.Value },
		        {
			    "cond", args =>
			    {
			        var clauses = args.Cast<Expression>().ToArray();

			        foreach (var clause in clauses.Take(args.Count() - 1))
			        {
				        var condition = Trampoline(clause.Head.Eval(this));
				        if (condition.To<bool>())
				        {
				            return InvokeCondClause(clause, condition);
				        }
			        }

			        var lastClause = clauses.Last();
			        var head = lastClause.Head.Quote(this);
			        if (head is SymbolValue && head.To<string>() == "else")
			        {
				        return InvokeCondClause(lastClause);
			        }

			        var lastCondition = Trampoline(lastClause.Head.Eval(this));
			        if (lastCondition.To<bool>())
			        {
				        return InvokeCondClause(lastClause, lastCondition);
			        }
				
			        return Null.Value;
			    }
		        },
		        {
			    "if", args =>
			    {
			        var condition = Trampoline(args[0].Eval(this)).To<bool>();
			        if (condition)
			        {
				        return new Closure(this, null, args[1], true);
			        }
			        if (args.Length > 2)
			        {
				        return new Closure(this, null, args[2], true);
			        }
			        return Null.Value;
			    }
		        },
		        {"let", Let},
		        {"let*", LetSequential},
		        {"set!", Set},
		        {
			    "begin", args =>
			    {
			        foreach (var arg in args.Take(args.Count() - 1))
			        {
				        Trampoline(arg.Eval(this));
			        }
			        return new Closure(this, null, args.Last(), true);
			    }
		        },
                        {"do", Do},
		        {"import", Import},
		        {"and", And},
		        {"or", Or},

		        // TODO for jit-compiler letrec and letrec* will have different implementations
		        {"letrec", Let},
		        {"letrec*", LetSequential},
		    };

            _globalFrame = new Frame();
            _callStack.Push(_globalFrame);
            CurrentFrame.BeginScope();
        }

        private Value Quasiquote(Node[] args)
        {
            var expression = args[0] as Expression;
            if (expression == null)
            {
                return args[0].Quote(this);
            }

            return new Cons(expression.Nodes.SelectMany(node =>
                {
                    var expressionNode = node as Expression;
                    if (expressionNode != null)
                    {
                        var value = expressionNode.Head.Quote(this);
                        if (value is SymbolValue)
                        {
                            var call = value.To<string>();
                            if (call == "unquote")
                            {
                                return new[] { expressionNode.Eval(this).ToExpression() };
                            }

                            if (call == "unquote-splicing")
                            {
                                var innerNode = expressionNode.Eval(this).ToExpression();
                                var innerExpressionNode = innerNode as Expression;
                                return innerExpressionNode != null ? innerExpressionNode.Nodes : new[] { innerNode };
                            }
                        }
                        return new [] { Quasiquote(new Node[] { expressionNode }).ToExpression() };
                    }
                    return new[] { node };
                }).Select(node => node.Quote(this)).ToArray());
        }

        private Value Set(Node[] args)
        {
            var name = args[0].Quote(this).To<string>();
            var value = Trampoline(args[1].Eval(this));
            CurrentFrame.Set(name, value);
            return Null.Value;
        }

        private Value Or(Node[] args)
        {
            foreach (var arg in args)
            {
                var value = Trampoline(arg.Eval(this));
                if (value.To<bool>())
                {
                    return value;
                }
            }
            return new Bool(false);
        }

        private Value And(Node[] args)
        {
            Value value = new Bool(true);
            foreach (var arg in args)
            {
                value = Trampoline(arg.Eval(this));
                if (!value.To<bool>())
                {
                    return new Bool(false);
                }
            }
            return value;
        }

        private Value Import(Node[] args)
        {
            var alias = args[0].Eval(this).To<string>();
            var module = ModuleAttribute.Find(alias);
            module.Import(_parser, this);
            return Null.Value;
        }

        private Value Do(Node[] args)
        {
            var variableClauses = args[0].Quote(this).To<IEnumerable<Value>>().Select(v => v.ToExpression()).Cast<Expression>();
            var testClause = args[1] as Expression;
            var body = new Expression(new[] { new Symbol(new SymbolValue("begin")) }.
                                      Concat(args.Skip(2)).ToArray());

            CurrentFrame.BeginScope();
            try
            {
                foreach (var clause in variableClauses)
                {
                    CurrentFrame.Bind(clause.Head.Quote(this).To<string>(), Trampoline(clause.Tail.First().Eval(this)));
                }            

                while(!Trampoline(testClause.Head.Eval(this)).To<bool>())
                {
                    Trampoline(body.Eval(this));
                    foreach (var clause in variableClauses)
                    {
                        if (clause.Tail.Count() > 1)
                        {
                            CurrentFrame.Set(clause.Head.Quote(this).To<string>(), Trampoline(clause.Tail.Skip(1).First().Eval(this)));
                        }
                    }       
                }
                return new Closure(this, null, new Expression(new[] { new Symbol(new SymbolValue("begin")) }.
                                                              Concat(testClause.Tail).ToArray()), true);
            }
            finally
            {
                CurrentFrame.EndScope();
            }
        }
        
        private Value Let(Node[] args)
        {
            var frameArgs = new List<string>();
            var frameValues = new List<Value>();
            var argNodes = new List<Node>();
            var name = string.Empty;

            if (args[0] is Symbol)
            {
                name = args[0].Quote(this).To<string>();
                args = args.Skip(1).ToArray();
            }
            
            foreach (var clause in args[0].Quote(this).To<IEnumerable<Value>>().Select(v => v.ToExpression()).Cast<Expression>())
            {
                frameArgs.Add(clause.Head.Quote(this).To<string>());
                frameValues.Add(Trampoline(clause.Tail.Single().Eval(this)));
                argNodes.Add(clause.Head);
            }

            CurrentFrame.BeginScope(frameArgs, frameValues);
            try
            {
                if (!string.IsNullOrEmpty(name))
                {
                    CurrentFrame.Bind(name, new Closure(this,
                                                        new Expression(argNodes),
                                                        new Expression(new[] { new Symbol(new SymbolValue("begin")) }.
                                                                       Concat(args.Skip(1)).ToArray()), true));
                }
                var result = new Closure(this, null, new Expression(new[] { new Symbol(new SymbolValue("begin")) }.
                                                                    Concat(args.Skip(1)).ToArray()), true);
                return result;
            }
            finally
            {
                CurrentFrame.EndScope();
            }
        }

        private Value LetSequential(Node[] args)
        {
            var name = string.Empty;
            var argNodes = new List<Node>();
            
            if (args[0] is Symbol)
            {
                name = args[0].Quote(this).To<string>();
                args = args.Skip(1).ToArray();
            }

            CurrentFrame.BeginScope();
            try
            {
                foreach (var clause in args[0].Quote(this).To<IEnumerable<Value>>().Select(v => v.ToExpression()).Cast<Expression>())
                {
                    argNodes.Add(clause.Head);
                    CurrentFrame.Bind(clause.Head.Quote(this).To<string>(), Trampoline(clause.Tail.Single().Eval(this)));
                }

                if (!string.IsNullOrEmpty(name))
                {
                    CurrentFrame.Bind(name, new Closure(this,
                                                        new Expression(argNodes),
                                                        new Expression(new[] { new Symbol(new SymbolValue("begin")) }.
                                                                       Concat(args.Skip(1)).ToArray()), true));
                }
                
                var result = new Closure(this, null, new Expression(new[] { new Symbol(new SymbolValue("begin")) }.
                                                                    Concat(args.Skip(1)).ToArray()), true);
                return result;
            }
            finally
            {
                CurrentFrame.EndScope();
            }
        }


        public void BeginFrame()
        {
            _callStack.Push(new Frame(_globalFrame));
        }

        public void EndFrame()
        {
            _callStack.Pop();
        }

        public Value Lookup(string name)
        {
            return CurrentFrame.Lookup(name);
        }

        public Value Trampoline(Value value)
        {
            var tailCall = value as Closure;
            while (tailCall != null && tailCall.IsTailCall)
            {
                value = InvokeClosure(tailCall, new Node[0]);
                tailCall = value as Closure;
            }
            return value;
        }

        public Value Invoke(Node head, IEnumerable<Node> args = null)
        {
            Value call;
            call = Trampoline(head.Eval(this));
            if (call is Undefined)
            {            
                if (head is Symbol)
                {
                    call = head.Quote(this);
                }
                else
                {
                    return Undefined.Value;
                }
            }

            if (call is SymbolValue)
            {
                var name = call.To<string>();
                if (_specialForms.ContainsKey(name))
                {
                    var value = _specialForms[name].Invoke(args != null ? args.ToArray() : new Node[] { });
                    if (CurrentFrame.IsGlobal)
                    {
                        value = Trampoline(value);
                    }

                    return value;
                }
            }
            else
            {
                var lambda = call as Closure;
                if (lambda != null)
                {
                    var value = InvokeClosure(lambda, args != null ? args.ToArray() : new Node[] { });
                    if (CurrentFrame.IsGlobal)
                    {
                        value = Trampoline(value);
                    }
                    return value;
                }
            }

            return Undefined.Value;
        }

        public Value Define(Node definition, Node body)
        {
            string[] def = definition is Expression
                ? definition.Quote(this).To<IEnumerable<Value>>().Select(value => value.To<string>()).ToArray()
                : new[] { definition.Quote(this).To<string>() };

            string name = def.First();
            string[] args = def.Skip(1).ToArray();

            if (definition is Expression)
            {
                CurrentFrame.Bind(name, new Closure(args, body));
            }
            else
            {
                CurrentFrame.Bind(name, body.Eval(this));
            }

            return Null.Value;
        }

        public Value DefineMacro(string name, Node args, Node body)
        {
            CurrentFrame.Bind(name, new Closure(this, args, body, false, true));
            return Null.Value;
        }

        public Value InvokeClosure(Closure closure, Node[] values)
        {
            var calculatedValues = closure.IsMacro
                ? values.Select(value => value.Quote(this)).ToArray()
                : values.Select(value => Trampoline(value.Eval(this))).ToArray();
            var arguments = closure.HasRestArg
                ? calculatedValues.Take(closure.Args.Count() - 1).Concat(new[]
                        {
                            new Cons(calculatedValues.Skip(closure.Args.Count() - 1).ToArray())
                        }).ToArray()
                : calculatedValues;

            if (!closure.IsMacro)
            {
                BeginFrame();
                CurrentFrame.Import(closure.Scopes);
            }
            CurrentFrame.BeginScope(closure.Args, arguments);                
            try
            {
                Value result;
                if (!closure.IsMacro)
                {
                    result = !closure.IsTailCall
                        ? new Closure(this, null, closure.Body, true)
                        : closure.Body.Eval(this);
                }
                else
                {
                    result = Trampoline(closure.Body.Eval(this));
                }
                return !(closure.IsMacro && _evalMacro) ? result : result.ToExpression().Eval(this);
            }
            finally
            {
                CurrentFrame.EndScope();                
                if (!closure.IsMacro)
                {
                    if (closure.Scopes != null)
                    {
                        for (var i = 0; i < closure.Scopes.Count(); i++)
                        {
                            CurrentFrame.EndScope();
                        }
                    }
                    EndFrame();
                }
            }
        }
    }
}