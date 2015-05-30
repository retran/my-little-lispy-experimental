using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using CorvusAlba.MyLittleLispy.Runtime;

using String = CorvusAlba.MyLittleLispy.Runtime.String;

namespace CorvusAlba.MyLittleLispy.Hosting
{
    sealed class RemoteAgent : IDisposable
    {
        private ScriptEngine _scriptEngine;
        private bool _synchronized;
        private bool _running;
        private int _port;
        private Task _task;
        
        public RemoteAgent(ScriptEngine scriptEngine, int port, bool synchronized = false)
        {
            _scriptEngine = scriptEngine;
            _synchronized = synchronized;
            _port = port;
        }
        
        public void Start()
        {
            _running = true;
            _task = Task.Run(Process);
        }
        
        private void Process()
        {
            var tcpListener = new TcpListener(IPAddress.Any, _port);
            tcpListener.Start(); 
            while (_running)
            {
                var client = tcpListener.AcceptSocket();
                var ns = new NetworkStream(client);
                using (var writer = new StreamWriter(ns))
                    using (var reader = new StreamReader(ns))
                    {
                        while (client.Connected && _running)
                        {
                            Value result = Null.Value;
                            var line = reader.ReadLine();
                            try
                            {
                                result = _scriptEngine.Evaluate(line);
                            }
                            catch (HaltException e)
                            {
                                result = new String("HALT " + e.Code);
                            }
                            writer.WriteLine(result);
                            writer.Flush();
                        }
                    }
            }
            tcpListener.Stop();
        }
        
        public void Stop()
        {
            if (_running)
            {
                _running = false;
                _task.Wait();            }
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
                Stop();                
            }
        }
    }
}