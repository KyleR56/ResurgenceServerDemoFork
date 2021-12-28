using System;
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
        }

        /// <summary>
        /// Sends a lidar report to Mission Control.
        /// </summary>
        public static void SendLidarReport(LidarSensor lidarSensor)
        {
            JArray jsonPoints = new JArray();
            foreach (double[] point in lidarSensor.Points)
                jsonPoints.Add(new JArray(point[0], point[1], point[2]));
            JObject lidarReport = new JObject()
            {
                ["type"] = "lidarReport",
                ["points"] = jsonPoints
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
        /// Instructs the simulator to set a simulated motor's target velocity.
        /// </summary>
        public static void SendSimMotorVelocityRequest(Motor motor)
        {
            JObject motorVelocityRequest = new JObject()
            {
                ["type"] = "simMotorVelocityRequest",
                ["motor"] = motor.Name,
                ["velocity"] = motor.TargetVelocity
            };
            Server.Instance.MessageSimulator(motorVelocityRequest);
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
                ["height"] = camera.StreamHeight
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
