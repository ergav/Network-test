using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class ClientTCP
{
    private TcpClient socket;
    private NetworkStream stream;
    private byte[] recieveBuffer;

    public const int dataBufferSize = 4096;
    
    public void Connect(string ip, int port)
    {
        socket = new TcpClient
        {
            ReceiveBufferSize = dataBufferSize,
            SendBufferSize = dataBufferSize
        };

        recieveBuffer = new byte[dataBufferSize];
        socket.BeginConnect(ip, port, ConnectionCallback, socket);
    }

    public void SendData(Packet packet)
    {
        byte[] packetbytes = packet.ToArray();
        
        try
        {
            if (socket != null)
            {
                stream.BeginWrite(packetbytes, 0, packetbytes.Length, null, null);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error sending data to server via TCP: " + ex);
        }
    }
    
    public void ConnectionCallback(IAsyncResult result)
    {
        socket.EndConnect(result);

        if (!socket.Connected)
        {
            return;
        }

        stream = socket.GetStream();

        stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallBack, null);
    }

    public void RecieveCallBack(IAsyncResult result)
    {
        try
        {
            int byteLength = stream.EndRead(result);
            if (byteLength <= 0)
            {
                return;
            }

            byte[] data = new byte[byteLength];
            Array.Copy(recieveBuffer, data, byteLength);
            HandleData(data);
            stream.BeginRead(recieveBuffer, 0, dataBufferSize, RecieveCallBack, null);
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    private void HandleData(byte[] data)
    {
        Packet packet = new(data);
        byte packetID = packet.GetByte();
        GameManager.instance.handlers[packetID](packet);
    }

    public void Disconnect()
    {
        stream.Close();
        socket.Close();
    }
}