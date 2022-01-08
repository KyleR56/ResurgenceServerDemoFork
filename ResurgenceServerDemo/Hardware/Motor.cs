using System;
using ResurgenceServerDemo.Network;

namespace ResurgenceServerDemo.Hardware
{
    /// <summary>
    /// A virtual representation of a motor on the rover.
    /// </summary>
    public class Motor
    {
        /// <summary>
        /// Determines the behavior of a motor.
        /// </summary>
        public enum RunMode
        {
            /// <summary>
            /// The motor will run with its target power.
            /// </summary>
            RunWithPower,
            /// <summary>
            /// The motor will try to reach its target position.
            /// </summary>
            RunToPosition
        }

        private RunMode _mode;
        private double _targetPower;
        private double _currentPower;
        private double _targetPosition;
        private double _currentPosition;
        private double _currentVelocity;

        /// <summary>
        /// Constructs a new virtual motor with the given name.
        /// </summary>
        public Motor(string name, bool hasEncoder)
        {
            Name = name;
            HasEncoder = hasEncoder;
            _mode = RunMode.RunWithPower;
            _targetPower = 0;
            _currentPower = 0;
            _targetPosition = 0;
            _currentPosition = 0;
            _currentVelocity = 0;
        }

        /// <summary>
        /// The name that identifies this motor.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The behavior of this motor.
        /// </summary>
        public RunMode Mode
        {
            get { return _mode; }
            set
            {
                if (value == _mode) return;
                if (value == RunMode.RunToPosition)
                    EnsureEncoder();
                _mode = value;
                switch (_mode)
                {
                    case RunMode.RunWithPower:
                        MessageSender.SendSimMotorPowerRequest(this);
                        break;
                    case RunMode.RunToPosition:
                        MessageSender.SendSimMotorPositionRequest(this);
                        break;
                }
            }
        }

        /// <summary>
        /// Whether this motor can read its position.
        /// </summary>
        public bool HasEncoder { get; }

        /// <summary>
        /// The power in [-1, 1] that this motor will try to run with.
        /// </summary>
        public double TargetPower
        {
            get { return _targetPower; }
            set
            {
                if (Math.Abs(value) > 1)
                    throw new ArgumentOutOfRangeException("|value| > 1");
                _targetPower = value;
                if (Mode == RunMode.RunWithPower)
                    MessageSender.SendSimMotorPowerRequest(this);
            }
        }

        /// <summary>
        /// The power in [-1, 1] of this motor as reported by the simulator.
        /// </summary>
        public double CurrentPower
        {
            get { return _currentPower; }
            set
            {
                _currentPower = value;
                MessageSender.SendMotorStatusReport(this);
            }
        }

        /// <summary>
        /// The position in degrees that this motor will try to reach.
        /// </summary>
        public double TargetPosition
        {
            get
            {
                EnsureEncoder();
                return _targetPosition;
            }
            set
            {
                EnsureEncoder();
                _targetPosition = value;
                if (Mode == RunMode.RunToPosition)
                    MessageSender.SendSimMotorPositionRequest(this);
            }
        }

        /// <summary>
        /// The position in degrees of this motor as reported by the simulator.
        /// </summary>
        public double CurrentPosition
        {
            get
            {
                EnsureEncoder();
                return _currentPosition;
            }
            set
            {
                EnsureEncoder();
                _currentPosition = value;
                MessageSender.SendMotorStatusReport(this);
            }
        }

        /// <summary>
        /// The velocity in degrees per second of this motor as reported by the
        /// simulator.
        /// </summary>
        public double CurrentVelocity
        {
            get
            {
                EnsureEncoder();
                return _currentVelocity;
            }
            set
            {
                EnsureEncoder();
                _currentVelocity = value;
                MessageSender.SendMotorStatusReport(this);
            }
        }

        /// <summary>
        /// Throws an exception if this motor does not have an encoder.
        /// </summary>
        private void EnsureEncoder()
        {
            if (!HasEncoder)
                throw new InvalidOperationException(Name + " has no encoder");
        }
    }
}
