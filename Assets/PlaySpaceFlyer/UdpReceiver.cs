using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using UniRx;

public sealed class UdpReceiver : IDisposable
{
    readonly UdpClient mUdp;

    readonly Subject<string> onReceived = new Subject<string>();
    public IObservable<string> OnReceivedAsObservable() => onReceived;

    public UdpReceiver(string ipAddress, int port)
    {
        var localEP = new IPEndPoint(IPAddress.Parse(ipAddress), port);
        mUdp = new UdpClient(localEP);
        mUdp.BeginReceive(OnReceived, mUdp);
    }

    public void Dispose()
    {
        onReceived.Dispose();
        mUdp.Dispose();
    }

    void OnReceived(IAsyncResult res)
    {
        var getUdp = (UdpClient) res.AsyncState;
        IPEndPoint ipEnd = null;

        try
        {
            var bytes = getUdp.EndReceive(res, ref ipEnd);
            onReceived.OnNext(Encoding.ASCII.GetString(bytes));
        }
        catch (Exception e)
        {
            onReceived.OnError(e);
            return;
        }
        getUdp.BeginReceive(OnReceived, getUdp);
    }
}