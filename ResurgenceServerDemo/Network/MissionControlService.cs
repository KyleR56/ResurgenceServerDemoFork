﻿using System;
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
                case "emergencyStopRequest":
                    MessageUtility.HandleEmergencyStopRequest(_rover, message);
                    break;
                case "driveRequest":
                    MessageUtility.HandleDriveRequest(_rover, message);
                    break;
                case "motorPowerRequest":
                    MessageUtility.HandleMotorPowerRequest(_rover, message);
                    break;
                case "cameraStreamOpenRequest":
                    MessageUtility.HandleCameraStreamOpenRequest(_rover, message);
                    break;
                case "cameraStreamCloseRequest":
                    MessageUtility.HandleCameraStreamCloseRequest(_rover, message);
                    break;
            }
        }
    }
}