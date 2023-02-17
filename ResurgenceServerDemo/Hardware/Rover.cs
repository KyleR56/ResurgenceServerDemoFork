using System;
using System.Collections.Generic;

namespace ResurgenceServerDemo.Hardware
{
    /// <summary>
    /// Virtual representation of the rover and its hardware components.
    /// </summary>
    public class Rover
    {
        public enum OperationMode
        {
            Teleoperation, Autonomous
        }

        private readonly IDictionary<string, Motor> _motors;
        private readonly IDictionary<string, Camera> _cameras;
        private bool _emergencyStopped;

        /// <summary>
        /// Constructs a new virtual representation of the rover.
        /// </summary>
        public Rover()
        {
            string[] motorNames = {
                "frontLeftWheel",
                "frontRightWheel",
                "rearLeftWheel",
                "rearRightWheel",
                "armBase",
                "shoulder",
                "elbow",
                "forearm",
                "wrist",
                "hand"
            };
            _motors = new Dictionary<string, Motor>();
            foreach (string motorName in motorNames)
            {
                _motors[motorName] = new Motor(motorName, true);
            }

            string[] cameraNames = { "front", "rear", "upperArm" };
            _cameras = new Dictionary<string, Camera>();
            foreach (string cameraName in cameraNames)
            {
                _cameras[cameraName] = new Camera(cameraName);
            }

            LidarSensor = new LidarSensor();

            _emergencyStopped = false;
        }

        public OperationMode Mode { get; set; }

        /// <summary>
        /// Whether the emergency stop is engaged.
        /// </summary>
        public bool EmergencyStopped
        {
            get { return _emergencyStopped; }
            set
            {
                _emergencyStopped = value;
                if (_emergencyStopped)
                {
                    foreach (Motor motor in _motors.Values)
                    {
                        motor.TargetPower = 0;
                        motor.Mode = Motor.RunMode.RunWithPower;
                    }
                }
            }
        }

        /// <summary>
        /// A collection containing all of the motors on this rover.
        /// </summary>
        public ICollection<Motor> Motors
        {
            get { return _motors.Values; }
        }

        /// <summary>
        /// A collection containing all of the cameras on this rover.
        /// </summary>
        public ICollection<Camera> Cameras
        {
            get { return _cameras.Values; }
        }

        /// <summary>
        /// Returns the motor on this rover with the given name.
        /// </summary>
        public Motor GetMotor(string motorName)
        {
            if (_motors.TryGetValue(motorName, out Motor motor))
            {
                return motor;
            }
            throw new ArgumentException("No such motor: " + motorName);
        }

        /// <summary>
        /// Returns the camera on this rover with the given name.
        /// </summary>
        public Camera GetCamera(string cameraName)
        {
            if (_cameras.TryGetValue(cameraName, out Camera camera))
            {
                return camera;
            }
            throw new ArgumentException("No such camera: " + cameraName);
        }

        public LidarSensor LidarSensor { get; }
    }
}
