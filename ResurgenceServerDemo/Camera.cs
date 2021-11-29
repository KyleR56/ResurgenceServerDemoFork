using System;
using Newtonsoft.Json.Linq;

namespace ResurgenceServerDemo
{
    /// <summary>
    /// A virtual representation of a camera on the rover.
    /// </summary>
    class Camera
    {
        private bool _isStreaming;
        private byte[] _streamData;

        /// <summary>
        /// Constructs a new virtual camera with the given name that does not
        /// stream by default.
        /// </summary>
        public Camera(string name)
        {
            Name = name;
            StreamFps = 0;
            StreamWidth = 0;
            StreamHeight = 0;
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
        public double StreamFps { get; set; }

        /// <summary>
        /// The width of this camera's stream.
        /// </summary>
        public double StreamWidth { get; set; }

        /// <summary>
        /// The height of this camera's stream.
        /// </summary>
        public double StreamHeight { get; set; }

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
                {
                    JObject cameraStreamOpenRequest = new JObject()
                    {
                        ["type"] = "cameraStreamOpenRequest",
                        ["camera"] = Name,
                        ["fps"] = StreamFps,
                        ["width"] = StreamWidth,
                        ["Height"] = StreamHeight
                    };
                    Server.Instance.MessageSimulator(cameraStreamOpenRequest);
                }
                else
                {
                    JObject cameraStreamCloseRequest = new JObject()
                    {
                        ["type"] = "cameraStreamCloseRequest",
                        ["camera"] = Name
                    };
                    Server.Instance.MessageSimulator(cameraStreamCloseRequest);
                }
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
                JObject cameraStreamReport = new JObject()
                {
                    ["type"] = "cameraStreamReport",
                    ["camera"] = Name,
                    ["data"] = Convert.ToBase64String(_streamData)
                };
                Server.Instance.MessageMissionControl(cameraStreamReport);
            }
        }
    }
}
