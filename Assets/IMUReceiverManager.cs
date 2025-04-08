using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

public class IMUReceiverManager : MonoBehaviour
{
    public static IMUReceiverManager Instance { get; private set; }

    private UdpClient udpClient;
    private Vector3 latestAcceleration = Vector3.zero;
    private bool udpInitialized = false;
    public int port = 5005;

    public Vector3 LatestAcceleration => latestAcceleration;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        StartCoroutine(InitializeUDP());
    }

    private IEnumerator InitializeUDP()
    {
        yield return new WaitForSeconds(1f);

        try
        {
            udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, port));
            udpClient.BeginReceive(ReceiveData, null);
            udpInitialized = true;
            Debug.Log($"✅ IMUReceiverManager started on port {port}");
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ IMUReceiverManager UDP Initialization Error: {e.Message}");
        }
    }

    private void ReceiveData(IAsyncResult result)
    {
        try
        {
            if (!udpInitialized || udpClient == null) return;

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            byte[] data = udpClient.EndReceive(result, ref sender);
            string received = Encoding.ASCII.GetString(data).Trim();
            string[] parts = received.Split(',');

            if (parts.Length >= 3 &&
                float.TryParse(parts[0], out float x) &&
                float.TryParse(parts[1], out float y) &&
                float.TryParse(parts[2], out float z))
            {
                latestAcceleration = new Vector3(x, y, z);
            }

            udpClient.BeginReceive(ReceiveData, null);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in ReceiveData: {e.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        udpClient?.Close();
        udpClient?.Dispose();
    }
}
