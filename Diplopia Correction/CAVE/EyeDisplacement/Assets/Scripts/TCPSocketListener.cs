using UnityEngine;
using System.Threading;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

// Using code from http://answers.unity3d.com/questions/12329/server-tcp-network-problem.html

public class TCPSocketListener : MonoBehaviour
{
    public string iPAddress = "192.168.2.67"; // "128.16.6.112";
    public int port = 13000;

    private EyeLinkDataConverter dataConverter;
    private bool mRunning;

    string msg = "";
    Thread mThread;
    TcpListener tcp_Listener = null;

    string[] subData = new string[0];

    void Start()
    {
        mRunning = true;
        ThreadStart ts = new ThreadStart(SayHello);
        mThread = new Thread(ts);
        mThread.Start();
        print("Thread done...");
    }

    void Awake()
    {
        dataConverter = GetComponent<EyeLinkDataConverter>();
    }

    public void stopListening()
    {
        mRunning = false;
    }

    void SayHello()
    {
        tcp_Listener = new TcpListener(System.Net.IPAddress.Parse(iPAddress), port);
        tcp_Listener.Start();
        print("Server Start");
        while (mRunning)
        {
            // check if new connections are pending, if not, be nice and sleep 100ms
            if (!tcp_Listener.Pending())
            {
                print("sleeping");
                Thread.Sleep(100);
            }
            else
            {
                TcpClient client = tcp_Listener.AcceptTcpClient();
                NetworkStream ns = client.GetStream();
                StreamReader reader = new StreamReader(ns);

                ns = client.GetStream();

                do
                {
                    msg = reader.ReadLine();
                    ns.Flush();

                    //set subData, so that it can be passed in update
                    subData = msg.Split(',');
                } while (msg !="fin");
                reader.Close();
                client.Close();
            }
        }
    }

    void OnApplicationQuit()
    {
        // stop listening thread
        stopListening();
        // wait fpr listening thread to terminate (max. 500ms)
        mThread.Join(500);
    }

    void FixedUpdate()
    {
        //this is on the main thread, so we can call funcitons now
        //if we have got new data, then send it
        if (subData.Length > 0)
            dataConverter.setData(subData);
    }
}
