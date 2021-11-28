using System;

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
            // Keep the main thread alive so the server isn't killed.
            while (server.IsListening) ;
        }
    }
}
