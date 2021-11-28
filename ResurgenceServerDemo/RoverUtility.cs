using Newtonsoft.Json.Linq;
using System;

namespace ResurgenceServerDemo
{
    /// <summary>
    /// Provides utilities for interfacing with a rover.
    /// </summary>
    static class RoverUtility
    {
        /// <summary>
        /// Handles a drive request sent from Mission Control to the rover.
        /// </summary>
        public static void HandleDriveRequest(Rover rover, JObject driveRequest)
        {
            double straight = (double)driveRequest["straight"];
            double steer = (double)driveRequest["steer"];

            // Normalize straight and steer so that motor power magnitudes do
            // not exceed 1.
            double magnitude = Math.Abs(straight) + Math.Abs(steer);
            if (magnitude > 1)
            {
                straight /= magnitude;
                steer /= magnitude;
            }

            double leftPower = straight + steer;
            double rightPower = straight - steer;

            rover.GetMotor("driveFrontLeft").Power = leftPower;
            rover.GetMotor("driveFrontRight").Power = rightPower;
            rover.GetMotor("driveRearLeft").Power = leftPower;
            rover.GetMotor("driveRearRight").Power = rightPower;
        }

        /// <summary>
        /// Handles a motor power request sent from Mission Control to the rover.
        /// </summary>
        public static void HandleMotorPowerRequest(Rover rover, JObject motorPowerRequest)
        {
            string motorName = (string)motorPowerRequest["motor"];
            double power = (double)motorPowerRequest["power"];
            rover.GetMotor(motorName).Power = power;
        }

        /// <summary>
        /// Handles a camera stream open request sent from Mission Control to the rover.
        /// </summary>
        public static void HandleCameraStreamOpenRequest(Rover rover, JObject cameraStreamOpenRequest)
        {
            string cameraName = (string)cameraStreamOpenRequest["camera"];
            Camera camera = rover.GetCamera(cameraName);
            camera.StreamFps = (double)cameraStreamOpenRequest["fps"];
            camera.StreamWidth = (int)cameraStreamOpenRequest["width"];
            camera.StreamHeight = (int)cameraStreamOpenRequest["height"];
            camera.IsStreaming = true;
        }

        /// <summary>
        /// Handles a camera stream close request sent from Mission Control to the rover.
        /// </summary>
        public static void HandleCameraStreamCloseRequest(Rover rover, JObject cameraStreamOpenRequest)
        {
            string cameraName = (string)cameraStreamOpenRequest["camera"];
            Camera camera = rover.GetCamera(cameraName);
            camera.IsStreaming = false;
        }

        /// <summary>
        /// Handles a camera stream report sent from the simulator to the rover.
        /// </summary>
        public static void HandleCameraStreamReport(Rover rover, JObject cameraStreamReport)
        {
            string cameraName = (string)cameraStreamReport["camera"];
            byte[] data = Convert.FromBase64String((string)cameraStreamReport["data"]);
            rover.GetCamera(cameraName).StreamData = data;
        }
    }
}
