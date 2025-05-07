namespace BitTorrent;

using System.Net;

public class Tracker
{
    public event EventHandler<List<IPEndPoint>> PeerListUpdated;
    public string Address;

    public Tracker(string address)
    {
        Address = address;
    }
}
