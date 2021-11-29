using System;
using Newtonsoft.Json.Linq;

namespace ResurgenceServerDemo
{
    /// <summary>
    /// A virtual representation of a motor on the rover.
    /// </summary>
    class Motor
    {
        private double _power;

        /// <summary>
        /// Constructs a new virtual motor with the given name and 0 power.
        /// </summary>
        public Motor(string name)
        {
            Name = name;
            _power = 0;
        }

        /// <summary>
        /// The name that identifies this motor.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// This motor's current power, which must be in the range [-1, 1].
        /// </summary>
        public double Power
        {
            get { return _power; }
            set
            {
                if (Math.Abs(value) > 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "|value| > 1");
                }
                _power = value;
                JObject motorPowerRequest = new JObject()
                {
                    ["type"] = "motorPowerRequest",
                    ["motor"] = Name,
                    ["power"] = _power
                };
                Server.Instance.MessageSimulator(motorPowerRequest);
            }
        }
    }
}
