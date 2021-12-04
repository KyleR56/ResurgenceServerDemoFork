﻿using System;
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

            // Inform the simulator of the rover's current state.

            foreach (Camera camera in _rover.GetCameras())
            {
                MessageUtility.SendCameraStreamOpenRequest(camera);
            }

            foreach (Motor motor in _rover.GetMotors())
            {
                MessageUtility.SendMotorPowerRequest(motor);
            }
        }

        protected override void OnMessage(MessageEventArgs e)
        {
            JObject message = JObject.Parse(e.Data);
            HandleMessage(message);
        }

        protected override void OnClose(CloseEventArgs e)
        {
            Console.WriteLine("Simulator disconnected.");

            foreach (Camera camera in _rover.GetCameras())
            {
                camera.StreamData = null;
            }
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