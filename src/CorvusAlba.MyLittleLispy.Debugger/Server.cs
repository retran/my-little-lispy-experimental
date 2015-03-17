using System;
using CorvusAlba.MyLittleLispy.Hosting;
using CorvusAlba.MyLittleLispy.Runtime;

namespace CorvusAlba.MyLittleLispy.Debugger
{
    public sealed class Server
    {
        private ScriptEngine _scriptEngine;
        private bool _synchronized;
        private int _port;
        
        public Server(ScriptEngine scriptEngine, int port = 12345, bool synchronized = false)
        {
            _scriptEngine = scriptEngine;
            _synchronized = synchronized;
            _port = port;
        }

        private Listen()
        {

        }
        
        public void Start()
        {

        }

        public void Stop()
        {

        }            
    }
}