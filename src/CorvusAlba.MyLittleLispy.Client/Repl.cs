using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using CorvusAlba.MyLittleLispy.Hosting;
using CorvusAlba.MyLittleLispy.Runtime;

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

        public int Loop()
        {
            Thread.Sleep(1000);
            var socketForServer = new TcpClient(_host, _port);
            var ns = socketForServer.GetStream();
            
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
                                var sw = new Stopwatch();
                            
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
            socketForServer.Close();
            return 0;
        }
    }
}