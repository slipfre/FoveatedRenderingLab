using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class ServerFinder : MonoBehaviour {

    private Socket ServerSocket;
    private IPEndPoint Clients;
    private IPEndPoint Server;
    private EndPoint epSender;

    string input;

    void Start()
    {
        ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        Server = new IPEndPoint(IPAddress.Any, 7001);
        ServerSocket.Bind(Server);
        Clients = new IPEndPoint(IPAddress.Any, 0);
        epSender = (EndPoint)Clients;
    }


	public void findServer(){
        byte[] getdata = new byte[1024]; //要接收的封包大小

        int recv;
        new Thread(() =>
        {
            int i = 1;
            i++;
            recv = ServerSocket.ReceiveFrom(getdata, ref epSender); 
            input = Encoding.UTF8.GetString(getdata, 0, recv);
            Debug.Log(input);
            ////关闭连接
            //Client.Close();

        }).Start();

    }

}
