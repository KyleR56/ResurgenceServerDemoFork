using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using ResurgenceServerDemo.Hardware;

namespace ResurgenceServerDemo.Network
{
    /// <summary>
    /// Provides utilities for sending messages to Mission Control and the
    /// simulator.
    /// </summary>
    public static class MessageSender
    {
        /// <summary>
        /// Sends a mounted peripheral report to Mission Control.
        /// </summary>
        public static void SendMountedPeripheralReport(string peripheral)
        {
            JObject mountedPeripheralReport = new JObject()
            {
                ["type"] = "mountedPeripheralReport",
                ["peripheral"] = peripheral
            };
            Server.Instance.MessageMissionControl(mountedPeripheralReport);
        }

        /// <summary>
        /// Sends a status report for the given motor to Mission Control.
        /// </summary>
        public static void SendMotorStatusReport(Motor motor)
        {
            JObject motorStatusReport = new JObject()
            {
                ["type"] = "motorStatusReport",
                ["motor"] = motor.Name,
                ["power"] = motor.CurrentPower
            };
            if (motor.HasEncoder)
            {
                motorStatusReport["position"] = motor.CurrentPosition;
                motorStatusReport["velocity"] = motor.CurrentVelocity;
            }
            else
            {
                motorStatusReport["position"] = null;
                motorStatusReport["velocity"] = null;
            }
            Server.Instance.MessageMissionControl(motorStatusReport);

            List<string> jointNames = new List<string> { "armBase", "shoulder", "elbow", "forearm", "wrist", "hand", };
            if (jointNames.Contains(motor.Name))
            {
                JObject jointPositionReport = new JObject()
                {
                    ["type"] = "jointPositionReport",
                    ["joint"] = motor.Name,
                    ["position"] = motor.CurrentPosition / 54.0 / 1000.0
                };
                Server.Instance.MessageMissionControl(jointPositionReport);
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
                ["data"] = camera.StreamData == null ? null : Convert.ToBase64String(camera.StreamData)
            };
            Server.Instance.MessageMissionControl(cameraStreamReport);
            JObject RoverPositionReport = new JObject()
            {
                ["type"] = "roverPositionReport",
                ["orientW"] = 1,
                ["orientX"] = 1,
                ["orientY"] = 1,
                ["orientZ"] = 1,
                ["posX"] = 1,
                ["posY"] = 1,
                ["posZ"] = 1,
                ["recency"] = 1
            };
            Server.Instance.MessageMissionControl(RoverPositionReport);
        }

        /// <summary>
        /// Sends a lidar report to Mission Control.
        /// </summary>
        public static void SendLidarReport(LidarSensor lidarSensor)
        {
            JObject[] jPoints = new JObject[lidarSensor.Points.Length];
            for (int i = 0; i < lidarSensor.Points.Length; i++)
            {
                LidarSensor.LidarPoint point = lidarSensor.Points[i];
                JObject jPoint = new JObject()
                {
                    ["x"] = point.R * Math.Cos(point.Theta),
                    ["y"] = point.R * Math.Sin(point.Theta)
                };
                jPoints[i] = jPoint;
            }
            JObject lidarReport = new JObject()
            {
                ["type"] = "lidarReport",
                ["points"] = new JArray(jPoints)
            };
            Server.Instance.MessageMissionControl(lidarReport);
        }

        /// <summary>
        /// Instructs the simulator to set a simulated motor's power.
        /// </summary>
        public static void SendSimMotorPowerRequest(Motor motor)
        {
            JObject motorPowerRequest = new JObject()
            {
                ["type"] = "simMotorPowerRequest",
                ["motor"] = motor.Name,
                ["power"] = motor.TargetPower
            };
            Server.Instance.MessageSimulator(motorPowerRequest);
        }

        /// <summary>
        /// Instructs the simulator to set a simulated motor's target position.
        /// </summary>
        public static void SendSimMotorPositionRequest(Motor motor)
        {
            JObject motorPositionRequest = new JObject()
            {
                ["type"] = "simMotorPositionRequest",
                ["motor"] = motor.Name,
                ["position"] = motor.TargetPosition
            };
            Server.Instance.MessageSimulator(motorPositionRequest);
        }

        /// <summary>
        /// Instructs the simulator to begin providing a camera stream, or to
        /// update the parameters of an existing camera stream.
        /// </summary>
        public static void SendSimCameraStreamOpenRequest(Camera camera)
        {
            JObject cameraStreamOpenRequest = new JObject()
            {
                ["type"] = "simCameraStreamOpenRequest",
                ["camera"] = camera.Name,
                ["fps"] = camera.StreamFps,
                ["width"] = camera.StreamWidth,
                ["height"] = camera.StreamHeight,
                ["intrinsicParameters"] = JArray.FromObject(new float[] { 649.3f, 0f, 335.4f, 0f, 647.7f, 247.8f, 0f, 0f, 1f })
            };
            Server.Instance.MessageSimulator(cameraStreamOpenRequest);
        }

        /// <summary>
        /// Instruct the simulator to stop streaming a simulated camera.
        /// </summary>
        public static void SendSimCameraStreamCloseRequest(Camera camera)
        {
            JObject cameraStreamCloseRequest = new JObject()
            {
                ["type"] = "simCameraStreamCloseRequest",
                ["camera"] = camera.Name
            };
            Server.Instance.MessageSimulator(cameraStreamCloseRequest);
        }
    }
}
