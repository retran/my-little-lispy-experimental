my-little-lispy
===============

Experimental implementation of simple scheme-like lisp dialect interpreter for using as DSL engine in .net software.

Features
--------

Examples
--------

```scheme
(define (square x) (* x x))
	
(square 10)

(define constx 5)
(define consty (+ constx 10))

(define (distance x y) (+ (square x) (square y)))
(distance 4 5)
(distance 2 3)

(distance constx consty)

(define (fact v) (if (< v 2) 1 (* v (fact (- v 1)))))
(fact 5)
(fact 10)            
```

Todo
----

- more types (maybe static typing?)
- more standart builtins (atom?, list?, etc)
- .net interoperability

- macro
- backquotes

- realtime compilation
- continuations

- documentation
