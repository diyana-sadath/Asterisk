using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Threading;

public class UDPListener : MonoBehaviour
{
    private UdpClient udpClient;
    private Thread receiveThread;
    public int port = 5000; // Make sure this matches your IMU data sender

    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("Listening on port " + port);
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] receivedBytes = udpClient.Receive(ref remoteEndPoint);
                string receivedText = Encoding.UTF8.GetString(receivedBytes);
                Debug.Log("Received: " + receivedText);
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving UDP data: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }
}
