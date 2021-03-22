using ExitGames.Client.Photon;

namespace PartyInc
{
    public interface IPlayerResultSender
    {
        /// <summary>
        /// Dont forget to subscribe and unsubscribe with:
        /// PhotonNetwork.NetworkingClient.EventReceived += SendMyResults;
        /// </summary>
        /// <param name="eventData"></param>
        void SendMyResults(EventData eventData);
    }
}
