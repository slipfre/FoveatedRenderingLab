using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public enum SessionResultCodes
{
    Success, Fail
};

public delegate void OnSessionSuccess();
public delegate void OnSessionFailed();

public abstract class BaseClient {

    public static string debugIP = "192.168.43.254";
    public static int debugPort = 27015;
    private Socket socket;
    private string ip;
    private int port;
    private byte[] recvBuff = new byte[1024];

    private OnSessionSuccess onSessionSuccess;
    private OnSessionFailed onSessionFailed;

    public BaseClient SetIP(string ip){
        this.ip = ip;
        return this;
    }

    public BaseClient SetPort(int port){
        this.port = port;
        return this;
    }

    public BaseClient SetSessionSuccessListener(OnSessionSuccess sessionSuccessListner){
        this.onSessionSuccess = sessionSuccessListner;
        return this;
    }

    public BaseClient SetSessionFailedListener(OnSessionFailed sessionFailedListener){
        this.onSessionFailed = sessionFailedListener;
        return this;
    }

    public BaseClient InitSocket(){
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        return this;
    }

    public void LaunchSession(){
        new Thread(() =>
        {
            // 建立连接
            IPAddress server = IPAddress.Parse(ip);
            socket.Connect(server, port);

            // 开始会话
            if (socket.Connected)
            {
                OnSessionFinished(OnSession());
            }

            //关闭连接
            socket.Close();

        }).Start();
    }

    public abstract SessionResultCodes OnSession();

    public void OnSessionFinished(SessionResultCodes resultCode){
        if (resultCode == SessionResultCodes.Success)
            onSessionSuccess();
        else if (resultCode == SessionResultCodes.Fail)
            onSessionFailed();
       
    }

    public int SendData(string data){
        int sendCount = 0;
        // 编码
        byte[] sendBuffer = Encoding.UTF8.GetBytes(data);
        // 发送
        int result;
        while(sendBuffer.Length > sendCount){
            result = socket.Send(sendBuffer);
            if (result == -1) return -1; // 改用异常更好
            sendCount += result;
        }

        return 1;
    }

    public int SendData(int data){
        return SendData(data.ToString());
    }

    public string RecvData(){
        int bytes = socket.Receive(recvBuff, recvBuff.Length, 0);
        if (bytes == -1) return null;   // 改用异常更好
        return Encoding.UTF8.GetString(recvBuff, 0, bytes);
    }

    public bool IsConnected(){
        return socket.Connected;
    }

    public void close(){
        socket.Close();
    }
}
