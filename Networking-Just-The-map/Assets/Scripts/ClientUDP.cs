using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class ClientUDP
{
    public UdpClient socket;
    public IPEndPoint endPoint;

    public void Connect()
    {
        endPoint = new IPEndPoint(IPAddress.Parse(Client.instance.ip), Client.instance.port);
        socket = new UdpClient(((IPEndPoint)Client.instance.tcp.socket.Client.LocalEndPoint).Port);
        
        socket.Connect(endPoint);

        socket.BeginReceive(ReceiveCallBack, null);
        
        Packet packet = new Packet();
        
        SendData(packet);
    }

    public void SendData(Packet packet)
    {
        byte[] packetBytes = packet.ToUdp(Client.instance.Id);
        
        try
        {
            socket?.BeginSend(packetBytes, packetBytes.Length, null, null);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }
    
    private void ReceiveCallBack(IAsyncResult result)
    {
        try
        {
            byte[] data = socket.EndReceive(result, ref endPoint);
            socket.BeginReceive(ReceiveCallBack, null);

            if (data.Length < 4)
            {
                Disconnect();
                return;
            }
            HandleData(data);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
            Disconnect();
        }
    }

    private void HandleData(byte[] data)
    {
        Packet packet = new Packet(data);
        byte packetID = packet.GetByte();
        GameManager.instance.handlers[packetID](packet);
    }

    public void Disconnect()
    {
        Client.instance.Disconnect();

        endPoint = null;
        socket = null;
    }
}