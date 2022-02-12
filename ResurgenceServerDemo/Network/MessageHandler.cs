using System;
using Newtonsoft.Json.Linq;
using ResurgenceServerDemo.Hardware;

namespace ResurgenceServerDemo.Network
{
    /// <summary>
    /// Provides utilities for handling messages received from Mission Control
    /// and the simulator.
    /// </summary>
    public static class MessageHandler
    {
        /// <summary>
        /// Handles an operation mode request sent from Mission Control to the
        /// rover.
        /// </summary
        public static void HandleOperationModeRequest(Rover _rover, JObject operationModeRequest)
        {
            string mode = (string)operationModeRequest["mode"];
            switch (mode)
            {
                case "teleoperation":
                    _rover.Mode = Rover.OperationMode.Teleoperation;
                    break;
                case "autonomous":
                    _rover.Mode = Rover.OperationMode.Autonomous;
                    break;
            }
        }

        /// <summary>
        /// Handles an emergency stop request sent from Mission Control to the
        /// rover.
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
        /// Handles a motor power request sent from Mission Control to the
        /// rover.
        /// </summary>
        public static void HandleMotorPowerRequest(Rover rover, JObject motorPowerRequest)
        {
            if (rover.EmergencyStopped)
            {
                return;
            }
            string motorName = (string)motorPowerRequest["motor"];
            double power = (double)motorPowerRequest["power"];
            Motor motor = rover.GetMotor(motorName);
            motor.TargetPower = power;
            motor.Mode = Motor.RunMode.RunWithPower;
        }

        /// <summary>
        /// Handles a motor position request sent from Mission Control to the
        /// rover.
        /// </summary>
        public static void HandleMotorPositionRequest(Rover rover, JObject motorPositionRequest)
        {
            if (rover.EmergencyStopped)
            {
                return;
            }
            string motorName = (string)motorPositionRequest["motor"];
            double position = (double)motorPositionRequest["position"];
            Motor motor = rover.GetMotor(motorName);
            motor.TargetPosition = position;
            motor.Mode = Motor.RunMode.RunToPosition;
        }

        /// <summary>
        /// Handles a camera stream open request sent from Mission Control to
        /// the rover.
        /// </summary>
        public static void HandleCameraStreamOpenRequest(Rover rover, JObject cameraStreamOpenRequest)
        {
            string cameraName = (string)cameraStreamOpenRequest["camera"];
            Camera camera = rover.GetCamera(cameraName);
            camera.StreamFps = 30;
            camera.StreamWidth = 500;
            camera.StreamHeight = 250;
            camera.IsStreaming = true;
        }

        /// <summary>
        /// Handles a camera stream close request sent from Mission Control to
        /// the rover.
        /// </summary>
        public static void HandleCameraStreamCloseRequest(Rover rover, JObject cameraStreamOpenRequest)
        {
            string cameraName = (string)cameraStreamOpenRequest["camera"];
            Camera camera = rover.GetCamera(cameraName);
            camera.IsStreaming = false;
        }

        /// <summary>
        /// Handles a motor status report sent from the simulator to the rover.
        /// </summary>
        public static void HandleSimMotorStatusReport(Rover rover, JObject motorStatusReport)
        {
            string motorName = (string)motorStatusReport["motor"];
            Motor motor = rover.GetMotor(motorName);

            double? currentPower = (double?)motorStatusReport["power"];
            double? currentPosition = (double?)motorStatusReport["position"];
            double? currentVelocity = (double?)motorStatusReport["velocity"];

            if (currentPower.HasValue)
                motor.CurrentPower = (double)currentPower;
            if (currentPosition.HasValue)
                motor.CurrentPosition = (double)currentPosition;
            if (currentVelocity.HasValue)
                motor.CurrentVelocity = (double)currentVelocity;
        }

        /// <summary>
        /// Handles a camera stream report sent from the simulator to the
        /// rover.
        /// </summary>
        public static void HandleSimCameraStreamReport(Rover rover, JObject cameraStreamReport)
        {
            string cameraName = (string)cameraStreamReport["camera"];
            byte[] data = Convert.FromBase64String((string)cameraStreamReport["data"]);
            rover.GetCamera(cameraName).StreamData = data;
        }

        /// <summary>
        /// Handles a lidar report sent from the simulator to rover.
        /// </summary>
        public static void HandleSimLidarReport(Rover rover, JObject lidarReport)
        {
            JArray jsonPoints = (JArray)lidarReport["points"];
            LidarSensor.LidarPoint[] points = new LidarSensor.LidarPoint[jsonPoints.Count];
            for (int i = 0; i < jsonPoints.Count; i++)
            {
                JObject jsonPoint = (JObject)jsonPoints[i];
                double r = (double)jsonPoint["r"];
                double theta = (double)jsonPoint["theta"];
                points[i] = new LidarSensor.LidarPoint(r, theta);
            }
            rover.LidarSensor.Points = points;
        }
    }
}
