using ResurgenceServerDemo.Network;

namespace ResurgenceServerDemo.Hardware
{
    /// <summary>
    /// A virtual representation of a camera on the rover.
    /// </summary>
    public class Camera
    {
        private double _streamFps;
        private int _streamWidth;
        private int _streamHeight;
        private bool _isStreaming;
        private byte[] _streamData;

        /// <summary>
        /// Constructs a new virtual camera with the given name that does not
        /// stream by default.
        /// </summary>
        public Camera(string name)
        {
            Name = name;
            _streamFps = 0;
            _streamWidth = 0;
            _streamHeight = 0;
            _isStreaming = false;
            _streamData = null;
        }

        /// <summary>
        /// The name that identifies this camera.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The frames per second of this camera's stream.
        /// </summary>
        public double StreamFps
        {
            get { return _streamFps; }
            set
            {
                _streamFps = value;
                if (_isStreaming)
                    // Inform the simulator about the new fps.
                    MessageSender.SendSimCameraStreamOpenRequest(this);
            }
        }

        /// <summary>
        /// The width of this camera's stream in pixels.
        /// </summary>
        public int StreamWidth
        {
            get { return _streamWidth; }
            set
            {
                _streamWidth = value;
                if (_isStreaming)
                    // Inform the simulator about the new width.
                    MessageSender.SendSimCameraStreamOpenRequest(this);
            }
        }

        /// <summary>
        /// The height of this camera's stream in pixels.
        /// </summary>
        public int StreamHeight
        {
            get { return _streamHeight; }
            set
            {
                _streamHeight = value;
                if (_isStreaming)
                    // Inform the simulator about the new height.
                    MessageSender.SendSimCameraStreamOpenRequest(this);
            }
        }

        /// <summary>
        /// Whether this camera is streaming data to Mission Control.
        /// </summary>
        public bool IsStreaming
        {
            get { return _isStreaming; }
            set
            {
                _isStreaming = value;
                // Inform the simulator whether we would like to receive a
                // simulated camera stream.
                if (_isStreaming)
                    MessageSender.SendSimCameraStreamOpenRequest(this);
                else
                    MessageSender.SendSimCameraStreamCloseRequest(this);
            }
        }

        /// <summary>
        /// This camera's stream's current frame encoded as a JPG image in the
        /// form of a byte array.
        /// </summary>
        public byte[] StreamData
        {
            get { return _streamData; }
            set
            {
                _streamData = value;
                MessageSender.SendCameraStreamReport(this);
            }
        }
    }
}
