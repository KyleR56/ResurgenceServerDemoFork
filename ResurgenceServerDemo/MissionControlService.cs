using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;

namespace ResurgenceServerDemo
{
    /// <summary>
    /// Provides behavior for handling the connection to the Mission Control
    /// WebSocket client.
    /// </summary>
    class MissionControlService : WebSocketBehavior
    {
        private Rover _rover;

        public void Initialize(Rover rover)
        {
            _rover = rover;
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Mission Control connected.");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            JObject message = JObject.Parse(e.Data);
            HandleMessage(message);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Mission Control disconnected.");
        }

        private void HandleMessage(JObject message)
        {
            string type = (string)message["type"];
            switch (type)
            {
                case "driveRequest":
                    RoverUtility.HandleDriveRequest(_rover, message);
                    break;
                case "motorPowerRequest":
                    RoverUtility.HandleMotorPowerRequest(_rover, message);
                    break;
                case "cameraStreamOpenRequest":
                    RoverUtility.HandleCameraStreamOpenRequest(_rover, message);
                    break;
                case "cameraStreamCloseRequest":
                    RoverUtility.HandleCameraStreamCloseRequest(_rover, message);
                    break;
            }
        }
    }
}
