using Newtonsoft.Json.Linq;
using System;
using ResurgenceServerDemo.Hardware;

namespace ResurgenceServerDemo.Network
{
    /// <summary>
    /// Provides utilities for handling and sending messages to Mission Control
    /// and the simulator.
    /// </summary>
    public static class MessageUtility
    {
        /// <summary>
        /// Handles an emergency stop request sent from Mission Control to the rover.
        /// </summary>
        public static void HandleEmergencyStopRequest(Rover rover, JObject emergencyStopRequest)
        {
            bool stop = (bool)emergencyStopRequest["stop"];
            rover.EmergencyStopped = stop;
        }

        /// <summary>
        /// Handles a drive request sent from Mission Control to the rover.
        /// </summary>
        public static void HandleDriveRequest(Rover rover, JObject driveRequest)
        {
            if (rover.EmergencyStopped)
            {
                return;
            }
            double straight = (double)driveRequest["straight"];
            double steer = (double)driveRequest["steer"];
            RoverUtility.Drive(rover, straight, steer);
        }

        /// <summary>
        /// Handles a motor power request sent from Mission Control to the rover.
        /// </summary>
        public static void HandleMotorPowerRequest(Rover rover, JObject motorPowerRequest)
        {
            if (rover.EmergencyStopped)
            {
                return;
            }

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

        /// <summary>
        /// Instructs the simulator to set a simulated motor's power.
        /// </summary>
        public static void SendMotorPowerRequest(Motor motor)
        {
            JObject motorPowerRequest = new JObject()
            {
                ["type"] = "motorPowerRequest",
                ["motor"] = motor.Name,
                ["power"] = motor.Power
            };
            Server.Instance.MessageSimulator(motorPowerRequest);
        }

        /// <summary>
        /// Instructs the simulator to begin providing a camera stream, or to
        /// update the parameters of an existing camera stream.
        /// </summary>
        public static void SendCameraStreamOpenRequest(Camera camera)
        {
            JObject cameraStreamOpenRequest = new JObject()
            {
                ["type"] = "cameraStreamOpenRequest",
                ["camera"] = camera.Name,
                ["fps"] = camera.StreamFps,
                ["width"] = camera.StreamWidth,
                ["height"] = camera.StreamHeight
            };
            Server.Instance.MessageSimulator(cameraStreamOpenRequest);
        }

        /// <summary>
        /// Instruct the simulator to stop streaming a simulated camera.
        /// </summary>
        public static void SendCameraStreamCloseRequest(Camera camera)
        {
            JObject cameraStreamCloseRequest = new JObject()
            {
                ["type"] = "cameraStreamCloseRequest",
                ["camera"] = camera.Name
            };
            Server.Instance.MessageSimulator(cameraStreamCloseRequest);
        }

        /// <summary>
        /// Informs the simulator of all camera streams requested by Mission
        /// Control. This should be invoked when the simulator first connects
        /// to the rover server.
        /// </summary>
        public static void ResendAllCameraStreamOpenRequests(Rover rover)
        {
            foreach (Camera camera in rover.GetCameras())
            {
                if (camera.IsStreaming)
                {
                    SendCameraStreamOpenRequest(camera);
                }
            }
        }

        /// <summary>
        /// Sends a frame of a camera stream to Mission Control.
        /// </summary>
        public static void SendCameraStreamReport(Camera camera)
        {
            JObject cameraStreamReport = new JObject()
            {
                ["type"] = "cameraStreamReport",
                ["camera"] = camera.Name,
                ["data"] = Convert.ToBase64String(camera.StreamData)
            };
            Server.Instance.MessageMissionControl(cameraStreamReport);
        }
    }
}
