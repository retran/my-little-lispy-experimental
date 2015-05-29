using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace CorvusAlba.MyLittleLispy.Hosting
{
    sealed class RemoteAgent : IDisposable
    {
        private ScriptEngine _scriptEngine;
        private bool _synchronized;
        private Thread _thread;
        private bool _running;
        private int _port;
        
        public RemoteAgent(ScriptEngine scriptEngine, int port, bool synchronized = false)
        {
            _scriptEngine = scriptEngine;
            _synchronized = synchronized;
            _thread = new Thread(Process);
            _port = port;
        }
        
        public void Start()
        {
            _running = true;
            _thread.Start();
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
                            var line = reader.ReadLine();
                            var result = _scriptEngine.Evaluate(line);
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
                _thread.Join();
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
                Stop();                
            }
        }
    }
}