using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System;
using System.Net.Sockets;
using System.Text;

public class SocketManager {

    #region 单例
    private static SocketManager _instance;
    public enum Character
    {
        None = 0,
        Server = 1,
        Client = 2,
    }
    public Character myCurrent;
    private SocketManager() { }
    public static SocketManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = new SocketManager();
            return _instance;
        }
    }
    #endregion

    public delegate void MyCallBack(string msg);
    public string ip = "127.0.0.1";
    public int port = 23456;
    #region 服务端
    Socket serverSocket;
    MyCallBack serverCallBack;
    byte[] serverBuffer;

    public void InitServer(MyCallBack callBack)
    {
        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        serverBuffer = new byte[1024];
        serverCallBack = callBack;

        IPEndPoint point = new IPEndPoint(IPAddress.Any, port);
        serverSocket.Bind(point);
        serverSocket.Listen(10);
        serverCallBack("服务器启动成功");
        serverSocket.BeginAccept(ServerAccept, serverSocket);
    }
    void ServerAccept(IAsyncResult ar)
    {
        serverSocket = ar.AsyncState as Socket;     //拿到当前连接服务器的Socket 对象
        Socket workingSocket = serverSocket.EndAccept(ar);         //将当前的Socket的全部信息交给一个新的Socket处理
        serverCallBack("客户端：" + serverSocket.AddressFamily + "已连接");
        workingSocket.BeginReceive(serverBuffer, 0, serverBuffer.Length, SocketFlags.None, ServerReceive, workingSocket);  //(数据收发缓存区,起始位置,长度,不分类型,全部接收, 接受消息之后的回调,当前Socket的状态)

        serverSocket.BeginAccept(ServerAccept, serverSocket);      //尾递归
    }
    //服务器接收消息之后的回调方法
    void ServerReceive(IAsyncResult ar)
    {
        Socket workingSorket = ar.AsyncState as Socket;
        int size = workingSorket.EndReceive(ar);   //拿到接收到的数据一共有多少字节
        string str = Encoding.UTF8.GetString(serverBuffer);    //将接收到的byte数组转换成字符串
        serverCallBack("接收到了" + size + "字节");
        serverCallBack(str);
        if (size == 0)
        {
            workingSorket.Shutdown(SocketShutdown.Both);
            workingSorket.Close();
            serverCallBack("连接已断开");
            return;
        }
        workingSorket.BeginReceive(serverBuffer, 0, serverBuffer.Length, SocketFlags.None, ServerReceive, workingSorket);   //尾递归
    }
    #endregion

    #region
    Socket clientSocket;
    MyCallBack clientCallBack;
    byte[] clientBuffer;

    //客户端连接服务器的方法
    public void ClientConnectServer(string ip, int port, MyCallBack callBack)
    {
        clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        clientBuffer = new byte[1024];
        clientCallBack = callBack;

        clientSocket.Connect(IPAddress.Parse(ip), port);      //连接服务器
        clientCallBack("连接服务器成功");
        clientSocket.BeginReceive(clientBuffer, 0, clientBuffer.Length, SocketFlags.None, ClientReceive, clientSocket);
    }

    void ClientReceive(IAsyncResult ar)
    {
        Socket WorkingSocket = ar.AsyncState as Socket;
        int size = WorkingSocket.EndReceive(ar);   //接收多少字节
        string msg = Encoding.UTF8.GetString(clientBuffer);
        clientCallBack("接收了" + size + "字节");
        clientCallBack(msg);
        if (size == 0)
        {
            WorkingSocket.Shutdown(SocketShutdown.Both);
            WorkingSocket.Close();
            clientCallBack("连接已断开");
            return;
        }
        clientSocket.BeginReceive(clientBuffer, size, clientBuffer.Length, SocketFlags.None, ClientReceive, WorkingSocket);
    }

    //客户端发送消息的方法
    public void ClientSendMsg(string msg)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(msg);
        clientSocket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, SendCallBack, clientSocket);
    }
    void SendCallBack(IAsyncResult ar)
    {
        Socket workingSocket = ar.AsyncState as Socket;
        workingSocket.EndSend(ar);
        clientCallBack("发送完成");
    }

    #endregion

    public void OnApplicationQuit()
    {
        //serverSocket.Shutdown(SocketShutdown.Both);
        //serverSocket.Close();
        //clientSocket.Disconnect(true);
        //clientSocket.Shutdown(SocketShutdown.Both);
        //clientSocket.Close();
    }
}
