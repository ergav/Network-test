using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dictionary<byte, PacketHandle> handlers = new();
    public delegate void PacketHandle(Packet packet);
    public static GameManager instance;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this);
        }
        
        handlers.Add((byte)Packet.PacketID.S_welcome, WelcomeRecieved);
    }

    public void WelcomeRecieved(Packet packet)
    {
        string message = packet.GetString();
        Debug.Log(message);
        int yourID = packet.GetInt();

        Packet welcomeRecieved = new Packet();
        welcomeRecieved.Add((byte)Packet.PacketID.C_welcomeReceived);
        welcomeRecieved.Add(yourID);
        Client.instance.tcp.SendData(welcomeRecieved);
    }
}