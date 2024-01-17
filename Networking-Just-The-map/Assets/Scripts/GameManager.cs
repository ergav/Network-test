using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Dictionary<byte, PacketHandle> handlers = new();
    public delegate void PacketHandle(Packet packet);
    public static GameManager instance;

    public GameObject playerPrefab;
    public GameObject localPlayerPrefab;
    public Dictionary<int, Player> playerList = new();

    public List<Action> actions = new();
    public List<Action> actionsCopy = new();

    public bool hasAction = false;
    
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
        handlers.Add((byte)Packet.PacketID.S_spawnPlayer, SpawnPlayer);
        handlers.Add((byte)Packet.PacketID.S_playerDisconnected, PlayerDisconnected);
        handlers.Add((byte)Packet.PacketID.S_playerPosition, PlayerPosition);
        handlers.Add((byte)Packet.PacketID.S_playerRotation, PlayerRotation);
    }

    private void Update()
    {
        if (hasAction)
        {
            actionsCopy.Clear();
            lock (actions)
            {
                actionsCopy.AddRange(actions);
                actions.Clear();
                hasAction = false;
            }

            foreach (Action action in actionsCopy)
            {
                action.Invoke();
            }
        }
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
        
        Client.instance.udp.Connect();
    }

    public void SpawnPlayer(Packet packet)
    {
        int id = packet.GetInt();
        string name = packet.GetString();
        Vector3 pos = packet.GetVector3();
        Quaternion rot = packet.GetQuaternion();

        GameObject prefabToSpawn = id == Client.instance.Id ? localPlayerPrefab : playerPrefab;
        
        lock (actions)
        {
            hasAction = true;
            actions.Add((() =>
            {
                Player newPlayer = Instantiate(prefabToSpawn, pos, rot).GetComponent<Player>();
                newPlayer.userName = name;
                playerList.Add(id, newPlayer);
            }));
        }
    }

    public void PlayerDisconnected(Packet packet)
    {
        int id = packet.GetInt();
        if (playerList.TryGetValue(id, out Player player))
        {
            lock (actions)
            {
                hasAction = true;
                actions.Add((() =>
                {
                    playerList.Remove(id);
                    Destroy(player.gameObject);
                }));
            }
        }
    }

    public void PlayerPosition(Packet packet)
    {
        int id = packet.GetInt();
        Vector3 position = packet.GetVector3();

        if (playerList.TryGetValue(id, out Player player))
        {
            player.targetPosition = position;
        }
    }

    public void PlayerRotation(Packet packet)
    {
        int id = packet.GetInt();
        if (id == Client.instance.Id)
        {
            return;
        }
        Quaternion rotation = packet.GetQuaternion();
        
        if (playerList.TryGetValue(id, out Player player))
        {
            player.transform.rotation = rotation;
        }
    }
}