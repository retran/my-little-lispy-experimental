using System;
using System.Collections.Concurrent;
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
        private bool _synchronous;
        private bool _running;
        private bool _waitingForSync;
        private int _port;
        private Task _task;
        private string _message;
        private AutoResetEvent _sync;

        public RemoteAgent(ScriptEngine scriptEngine, int port, bool synchronous = true)
        {
            _scriptEngine = scriptEngine;
            _synchronous = synchronous;
            _port = port;
            if (!_synchronous)
            {
                _sync = new AutoResetEvent(false);
            }
        }

        public void Start()
        {
            _running = true;
            _task = Task.Run(new Action(Process));
        }

        private Value Evaluate(string line)
        {
            try
            {
                return _scriptEngine.Evaluate(line);
            }
            catch (HaltException e)
            {
                return new String("HALT " + e.Code);
            }
            catch (Exception e)
            {
                return new String("EXCEPTION " + e.Message);
            }
        }

        private async void Process()
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
                        _message = await reader.ReadLineAsync();
                        if (_synchronous)
                        {
                            _message = Evaluate(_message).ToString();
                        }
                        else
                        {
                            _sync.Reset();
                            _waitingForSync = true;
                            _sync.WaitOne();
                        }
                        await writer.WriteLineAsync(_message);
                        await writer.FlushAsync();
                    }
                }
            }
            tcpListener.Stop();
        }

        public void Sync()
        {
            if (_waitingForSync)
            {
                _message = Evaluate(_message).ToString();
                _sync.Set();
                _waitingForSync = false;
            }
        }

        public void Stop()
        {
            if (_running)
            {
                _running = false;
                _task.Wait();
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