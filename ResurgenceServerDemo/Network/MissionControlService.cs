using System;
using WebSocketSharp;
using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;
using ResurgenceServerDemo.Hardware;

namespace ResurgenceServerDemo.Network
{
    /// <summary>
    /// Provides behavior for handling the connection to the Mission Control
    /// WebSocket client.
    /// </summary>
    public class MissionControlService : WebSocketBehavior
    {
        private Rover _rover;

        public void Initialize(Rover rover)
        {
            _rover = rover;
        }

        protected override void OnOpen()
        {
            Console.WriteLine("Mission Control connected.");
            MessageSender.SendMountedPeripheralReport("arm");
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            JObject message = JObject.Parse(e.Data);
            HandleMessage(message);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Mission Control disconnected.");

            // Stop all camera streams.
            foreach (Camera camera in _rover.Cameras)
            {
                camera.IsStreaming = false;
            }
        }

        private void HandleMessage(JObject message)
        {
            string type = (string)message["type"];
            switch (type)
            {
                case "operationModeRequest":
                    MessageHandler.HandleOperationModeRequest(_rover, message);
                    break;
                case "emergencyStopRequest":
                    MessageHandler.HandleEmergencyStopRequest(_rover, message);
                    break;
                case "driveRequest":
                    MessageHandler.HandleDriveRequest(_rover, message);
                    break;
                case "jointPowerRequest":
                    MessageHandler.HandleJointPowerRequest(_rover, message);
                    break;
                case "jointPositionRequest":
                    MessageHandler.HandleJointPositionRequest(_rover, message);
                    break;
                case "cameraStreamOpenRequest":
                    MessageHandler.HandleCameraStreamOpenRequest(_rover, message);
                    break;
                case "cameraStreamCloseRequest":
                    MessageHandler.HandleCameraStreamCloseRequest(_rover, message);
                    break;
                default:
                    Console.Error.WriteLine("Unknown message type: " + type);
                    break;
            }
        }
    }
}
