using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;

namespace ResurgenceServerDemo
{
    /// <summary>
    /// Provides behavior for handling the connection to the Simulator
    /// WebSocket client.
    /// </summary>
    class SimulatorService : WebSocketBehavior
    {
        private Rover _rover;

        public void Initialize(Rover rover)
        {
            _rover = rover;
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Simulator connected.");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            JObject message = JObject.Parse(e.Data);
            HandleMessage(message);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Simulator disconnected.");
        }

        private void HandleMessage(JObject message)
        {
            string type = (string)message["type"];
            switch (type)
            {
                case "cameraStreamReport":
                    RoverUtility.HandleCameraStreamReport(_rover, message);
                    break;
            }
        }
    }
}
