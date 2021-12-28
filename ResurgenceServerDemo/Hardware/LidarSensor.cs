using ResurgenceServerDemo.Network;

namespace ResurgenceServerDemo.Hardware
{
    public class LidarSensor
    {
        public double[][] _points;

        public LidarSensor()
        {
            _points = null;
        }

        public double[][] Points
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
