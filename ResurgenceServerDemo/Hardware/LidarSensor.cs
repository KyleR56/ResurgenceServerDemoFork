using ResurgenceServerDemo.Network;

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

            public double R { get; }

            public double Theta { get; }
        }

        private LidarPoint[] _points;

        public LidarSensor()
        {
            _points = null;
        }

        public LidarPoint[] Points
        {
            get { return _points; }
            set
            {
                _points = value;
                MessageSender.SendLidarReport(this);
            }
        }
    }
}
