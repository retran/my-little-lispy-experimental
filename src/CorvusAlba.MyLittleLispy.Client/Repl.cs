using System;
using System.Diagnostics;
using System.Linq;
using CorvusAlba.MyLittleLispy.Hosting;
using CorvusAlba.MyLittleLispy.Runtime;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;

namespace CorvusAlba.MyLittleLispy.Client
{
	public sealed class Server : IDisposable
	{
		public static readonly string DefaultPipeName = "mylittlelispy";

		private ScriptEngine _scriptEngine;
		private bool _synchronized;
		private Thread _debuggerThread;
		private bool _running = false;

		public Server(ScriptEngine scriptEngine, string name = "mylittlelispy", bool synchronized = false)
		{
                    _scriptEngine = scriptEngine;
                    _synchronized = synchronized;
                    _debuggerThread = new Thread(new ThreadStart(Process));
		}

		public void Start()
		{
                    _running = true;
                    _debuggerThread.Start();
		}

		private void Process()
		{
                    var tcpListener = new TcpListener(55555);
                    tcpListener.Start(); 
                    Socket client = tcpListener.AcceptSocket();
                    
                    NetworkStream ns = new NetworkStream(client);
                    
                    using (var writer = new StreamWriter(ns))
                        using (var reader = new StreamReader(ns))
                        {
                            while (true)
                            {
                                var line = reader.ReadLine();
                                var result = _scriptEngine.Evaluate(line);
                                writer.WriteLine(result.ToString());
                                writer.Flush();
                            }
                        }
		}

		public void Stop()
		{
			if (_running)
			{
				_running = false;
				_debuggerThread.Abort(); // TODO use eventhandlers
			}
		}            

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.Stop();                
			}
		}
	}

	internal class Repl
    {
        private readonly ScriptEngine _engine;
        private readonly Server _server;

        public Repl(ScriptEngine engine)
        {
            _engine = engine;
            _server = new Server(_engine);
        }

        public int Loop()
        {
            _server.Start();
            Thread.Sleep(1000);

            TcpClient socketForServer = null;
            try
            {
                socketForServer = new TcpClient("localHost", 55555);
            }
            catch
            {
                Console.WriteLine("Failed to connect to server at {0}:999", "localhost");
            }

            NetworkStream ns = socketForServer.GetStream();
            
            using (var writer = new StreamWriter(ns))
                using (var reader = new StreamReader(ns))
                {
                    while (true)
                    {
                        Console.Write(" > ");
                        try
                        {
                            var line = Console.ReadLine();
                            
                            if (line == null)
                            {
                                line = "(halt 0)";
                            }
                            
                            while (true)
                            {
                                var count = line.Count(c => c == '(') - line.Count(c => c == ')');
                                if (count == 0)
                                {
                                    break;
                                }
                                
                                Console.Write(" ... ");
                                line = line + Console.ReadLine();
                            }
                            
                            Stopwatch sw = new Stopwatch();
                            
                            sw.Start();
                            writer.WriteLine(line);
                            writer.Flush();
                            var value = reader.ReadLine();
                            sw.Stop();
                            
                            TimeSpan ts = sw.Elapsed;
                            string elapsedTime = string.Format("{0:00}:{1:00}:{2:00}:{3:000}",
                                                               ts.Hours, ts.Minutes, ts.Seconds,
                                                               ts.Milliseconds);
                    
                            Console.WriteLine(" => {0}", value);
                            Console.WriteLine("(elapsed {0})", elapsedTime);
                        
                        }
                        catch (HaltException e)
                        {
                            return e.Code;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
            }
        }
    }
}