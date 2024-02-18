using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class SocketController : MonoBehaviour
{
    private Socket socket;

    public LocalServerInfoSO localServer;

    private byte[] ibuffer = new byte[2048];
    private byte[] obuffer = new byte[2048];


    private OnlineManager onlineManager;

    void Awake()
    {
        onlineManager = GetComponent<OnlineManager>();
    }

    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream,
            ProtocolType.Tcp);
        IPEndPoint remoteEP = new IPEndPoint(IPAddress.Parse(localServer.IP), localServer.Port);
        socket.Connect(remoteEP);

        StartCoroutine(OnDataReceived());
    }

    public void Send(string str)
    {
        byte[] strBuffer = Encoding.ASCII.GetBytes(str + "$");
        strBuffer.CopyTo(obuffer, 0);
        socket.Send(obuffer, strBuffer.Length, SocketFlags.None);
    }

    private IEnumerator OnDataReceived()
    {
        while (true)
        {
            if (socket.Available > 0)
            {
                int bytesReceived = socket.Available;
                socket.Receive(ibuffer, bytesReceived, SocketFlags.None);
                
                onlineManager.ParseMessages(ibuffer, bytesReceived);
            }
            yield return null;
        }
    }
}
