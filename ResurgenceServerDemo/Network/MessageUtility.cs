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
        /// Handles a motor velocity request sent from Mission Control to the 
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
        /// Handles a motor position request sent from Mission Control to the
        /// rover.
        /// </summary>
        public static void HandleMotorVelocityRequest(Rover rover, JObject motorVelocityRequest)
        {
            if (rover.EmergencyStopped)
            {
                return;
            }
            string motorName = (string)motorVelocityRequest["motor"];
            double position = (double)motorVelocityRequest["position"];
            Motor motor = rover.GetMotor(motorName);
            motor.TargetVelocity = position;
            motor.Mode = Motor.RunMode.RunWithVelocity;
        }

        /// <summary>
        /// Handles a camera stream open request sent from Mission Control to
        /// the rover.
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
    }
}
