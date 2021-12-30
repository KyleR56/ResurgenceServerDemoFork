namespace ResurgenceServerDemo.Hardware
{
    public class LidarSensor
    {
        public struct LidarPoint
        {
            public LidarPoint(double r, double theta)
            {
                R = r;
                Theta = theta;
            }

            public double R { get; set; }

            public double Theta { get; set; }
        }

        public LidarSensor()
        {
            Points = null;
        }

        public LidarPoint[] Points { get; set; }
    }
}
