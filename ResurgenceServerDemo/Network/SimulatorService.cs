using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;
using ResurgenceServerDemo.Hardware;

namespace ResurgenceServerDemo.Network
{
    /// <summary>
    /// Provides behavior for handling the connection to the Simulator
    /// WebSocket client.
    /// </summary>
    public class SimulatorService : WebSocketBehavior
    {
        private Rover _rover;

        public void Initialize(Rover rover)
        {
            _rover = rover;
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Simulator connected.");
            MessageUtility.ResendAllCameraStreamOpenRequests(_rover);
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
                    MessageUtility.HandleCameraStreamReport(_rover, message);
                    break;
            }
        }
    }
}
