using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Client : MonoBehaviour
{
    public static Client instance;

    public string playerName = "Player1";
    
    public string ip = "127.0.0.1";
    public int port = 25565;
    public int Id = 0;

    public ClientTCP tcp = new();
    public ClientUDP udp = new();
    
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
    }

    public void SetName(string name)
    {
        playerName = name;
    }
    
    public void Connect()
    {
        tcp.Connect(ip, port);
    }

    public void JoinGame()
    {
        Packet packet = new Packet();
        packet.Add((byte)Packet.PacketID.C_spawnPlayer);
        packet.Add(playerName);
        tcp.SendData(packet);
    }

    public void Disconnect()
    {
        tcp.Disconnect();

        if (GameManager.instance.playerList.TryGetValue(Id, out Player player))
        {
            GameManager.instance.playerList.Remove(Id);
            Destroy(player.gameObject);
        }
    }
}