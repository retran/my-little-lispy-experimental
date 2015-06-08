using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace CorvusAlba.MyLittleLispy.Client
{
    class Repl
    {
        private readonly string _host;
        private readonly int _port;

        public Repl(string host, int port)
        {
            _host = host;
            _port = port;
        }

        public async Task<int> Loop()
        {
            var socketForServer = new TcpClient(_host, _port);
            var ns = socketForServer.GetStream();
            using (var writer = new StreamWriter(ns))
            using (var reader = new StreamReader(ns))
            {
                while (true)
                {
                    Console.Write(" > ");
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
                        Console.Write(" ... ");
                        var newpart = Console.ReadLine();
                        if (newpart != null)
                        {
                            line = line.TrimEnd('\n', '\r') + " " + newpart;
                        }
                    }
                    if (!string.IsNullOrEmpty(line))
                    {
                        await writer.WriteLineAsync(line);
                        await writer.FlushAsync();
                        var value = await reader.ReadLineAsync();
                        Console.WriteLine(" => {0}", value);
                    }
                }
            }
            socketForServer.Close();
            return 0;
        }
    }
}

