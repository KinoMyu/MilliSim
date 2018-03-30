using System.Threading.Tasks;
using JetBrains.Annotations;
using OpenMLTD.Piyopiyo;
using OpenMLTD.Piyopiyo.Net;
using OpenMLTD.TheaterDays.Subsystems.Bvs.Models;

namespace OpenMLTD.TheaterDays.Subsystems.Bvs {
    internal sealed class TDSimulatorClient : JsonRpcClient {

        internal TDSimulatorClient([NotNull] TDCommunication communication) {
            _communication = communication;
        }

        internal Task SendInitializedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.General_SimInitialized);
        }

        internal Task SendLaunchedNotification() {
            var serverUri = _communication.EditorServerUri;

            if (serverUri == null) {
                return Task.FromResult(0);
            }

            var simulatorServerPort = _communication.Server.EndPoint.Port;

            var @params = new GeneralSimLaunchedNotificationParams {
                SimulatorServerUri = $"http://localhost:{simulatorServerPort}/"
            };

            return CallAsync(serverUri, CommonProtocolMethodNames.General_SimLaunched, @params);
        }

        internal Task SendPlayingNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Playing);
        }

        internal Task SendTickNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Tick);
        }

        internal Task SendPausedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Paused);
        }

        internal Task SendStoppedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_Stopped);
        }

        internal Task SendExitedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.General_SimExited);
        }

        internal Task SendReloadedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Edit_Reloaded);
        }

        internal Task SendSeekingCompletedNotification() {
            return SendNotificationWithEmptyBody(CommonProtocolMethodNames.Preview_SeekingCompleted);
        }

        private Task SendNotificationWithEmptyBody([NotNull] string methodName) {
            var serverUri = _communication.EditorServerUri;

            if (serverUri == null) {
                return Task.FromResult(0);
            }

            return CallAsync(serverUri, methodName);
        }

        private readonly TDCommunication _communication;

    }
}