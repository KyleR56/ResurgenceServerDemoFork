using System;

namespace ResurgenceServerDemo.Hardware
{
    /// <summary>
    /// Provides utilities for interfacing with rover hardware.
    /// </summary>
    public static class RoverUtility
    {
        /// <summary>
        /// Sets the power of the rover's drive motors so that the rover drives
        /// with the given straight and steer values.
        /// </summary>
        public static void Drive(Rover rover, double straight, double steer)
        {
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

            Motor frontLeftWheel = rover.GetMotor("frontLeftWheel");
            frontLeftWheel.TargetPower = leftPower;
            frontLeftWheel.Mode = Motor.RunMode.RunWithPower;

            Motor frontRightWheel = rover.GetMotor("frontRightWheel");
            frontRightWheel.TargetPower = rightPower;
            frontRightWheel.Mode = Motor.RunMode.RunWithPower;

            Motor rearLeftWheel = rover.GetMotor("rearLeftWheel");
            rearLeftWheel.TargetPower = leftPower;
            rearLeftWheel.Mode = Motor.RunMode.RunWithPower;

            Motor rearRightWheel = rover.GetMotor("rearRightWheel");
            rearRightWheel.TargetPower = rightPower;
            rearRightWheel.Mode = Motor.RunMode.RunWithPower;
        }
    }
}
