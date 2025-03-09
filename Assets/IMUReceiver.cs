using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class IMUReceiver : MonoBehaviour
{
    public string serverIP = "192.168.1.12"; // Your PC's local IP address
    public int port = 5000; // Use the same port as HyperIMU
    private UdpClient udpClient;
    private Thread receiveThread;
    private float roll = 0f;

    void Start()
    {
        udpClient = new UdpClient(port);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
        Debug.Log("IMUReceiver started, listening on port " + port);
    }

    void ReceiveData()
    {
        IPEndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, port);
        while (true)
        {
            try
            {
                byte[] data = udpClient.Receive(ref remoteEndPoint);
                string receivedString = Encoding.UTF8.GetString(data);
                string[] values = receivedString.Split(',');

                if (values.Length >= 4) // Ensure at least 4 values are received
                {
                    if (float.TryParse(values[2], out roll)) // Assuming roll is at index 2
                    {
                        Debug.Log("Received Roll: " + roll); // ✅ Debug output
                    }
                }
            }
            catch (ThreadAbortException)
            {
                Debug.Log("IMU Receiver thread aborted.");
                break;
            }
            catch (Exception ex)
            {
                Debug.Log("Error receiving data: " + ex.Message);
            }
        }
    }

    public float GetRoll()
    {
        return roll;
    }

    void OnApplicationQuit()
    {
        receiveThread.Abort();
        udpClient.Close();
    }
}
