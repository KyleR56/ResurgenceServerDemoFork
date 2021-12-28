using WebSocketSharp.Server;
using Newtonsoft.Json.Linq;
using ResurgenceServerDemo.Hardware;

namespace ResurgenceServerDemo.Network
{
    /// <summary>
    /// Provides a WebSocket server that connects Mission Control with the
    /// simulator.
    /// </summary>
    public class Server
    {
        private static Server s_instance;

        private const string MissionControlPath = "/mission-control";
        private const string SimulatorPath = "/simulator";

        private WebSocketServer _server;

        /// <summary>
        /// Constructs a new server that runs on the given port.
        /// </summary>
        public Server(int port, Rover rover)
        {
            _server = new WebSocketServer(port);

            _server.AddWebSocketService<MissionControlService>(
                MissionControlPath,
                (service) => service.Initialize(rover)
            );

            _server.AddWebSocketService<SimulatorService>(
                SimulatorPath,
                (service) => service.Initialize(rover)
            );

            s_instance = this;
        }

        /// <summary>
        /// The current server instance.
        /// </summary>
        public static Server Instance
        {
            get { return s_instance; }
        }

        /// <summary>
        /// Whether this server is listening.
        /// </summary>
        public bool IsListening
        {
            get { return _server.IsListening; }
        }

        /// <summary>
        /// Starts the server.
        /// </summary>
        public void Start()
        {
            _server.Start();
        }

        /// <summary>
        /// Stops the server.
        /// </summary>
        public void Stop()
        {
            _server.Stop();
        }

        /// <summary>
        /// Sends a message to the Mission Control WebSocket client.
        /// Effectively does nothing if Mission Control is not connected.
        /// </summary>
        public void MessageMissionControl(JObject message)
        {
            _server.WebSocketServices[MissionControlPath].Sessions.Broadcast(message.ToString(Newtonsoft.Json.Formatting.None));
        }

        /// <summary>
        /// Sends a message to the simulator WebSocket client. Effectively does
        /// nothing if the simulator is not connected.
        /// </summary>
        public void MessageSimulator(JObject message)
        {
            _server.WebSocketServices[SimulatorPath].Sessions.Broadcast(message.ToString(Newtonsoft.Json.Formatting.None));
        }
    }
}
