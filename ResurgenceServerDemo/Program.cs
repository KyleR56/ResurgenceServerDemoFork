using System;
using System.Threading;

namespace ResurgenceServerDemo
{
    class Program
    {
        /// <summary>
        /// Port on which the WebSocket server runs.
        /// </summary>
        private const int Port = 3001;

        private static void Main()
        {
            Rover rover = new Rover();
            Server server = new Server(Port, rover);
            server.Start();
            Console.WriteLine("Server started on port {0}.", Port);
            Console.WriteLine("Press any key to stop the server.");
            Console.ReadKey();
            server.Stop();
            Console.WriteLine("Server stopped.");
        }
    }
}
