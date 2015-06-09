using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CorvusAlba.MyLittleLispy.Client
{
    class Repl
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _remote;
        private readonly string _promptLine;

        public Repl(string host, int port, bool remote = false)
        {
            _host = host;
            _port = port;
            _remote = remote;
            _promptLine = _remote
                ? string.Format("mll@{0}:{1}> ", _host, _port)
                : "mll> ";
        }

        public async Task<int> Loop()
        {
            var socketForServer = new TcpClient(_host, _port);
            try
            {
                using (var ns = socketForServer.GetStream())
                using (var writer = new StreamWriter(ns))
                using (var reader = new StreamReader(ns))
                {
                    while (true)
                    {
                        var lines = new List<string>();
                        Console.Write(_promptLine);
                        var line = Console.ReadLine();
                        if (line == null)
                        {
                            break;
                        }
                        while (true)
                        {
                            var count = line.Count(c => c == '(') - line.Count(c => c == ')');
                            if (count == 0)
                            {
                                break;
                            }
                            var newpart = Console.ReadLine();
                            if (newpart != null)
                            {
                                line = line.TrimEnd('\n', '\r') + " " + newpart;
                            }
                        }

                        if (!string.IsNullOrEmpty(line))
                        {
                            lines.Add(line);
                        }

                        foreach (var l in lines)
                        {
                            await writer.WriteLineAsync(line);
                            await writer.FlushAsync();
                            var value = await reader.ReadLineAsync();
                            Console.WriteLine("{0}", value);
                        }
                    }

                }
                socketForServer.Close();
            }
            catch (IOException)
            {
                Console.WriteLine("Socket disconnected.");
            }
            return 0;
        }
    }
}

