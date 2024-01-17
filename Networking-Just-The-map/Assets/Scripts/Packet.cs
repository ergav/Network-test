using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Packet
{
    private List<byte> writableData = new List<byte>();
    private byte[] data;
    private int readPos = 0;

    private int packetLength = 0;
    
    public Packet() {}

    public Packet(byte[] data)
    {
        this.data = data;
        packetLength = GetInt();
    }

    public byte[] ToArray()
    {
        writableData.InsertRange(0, BitConverter.GetBytes(writableData.Count));
        return writableData.ToArray();
    }

    public byte[] ToUdp(int value)
    {
        writableData.InsertRange(0, BitConverter.GetBytes(writableData.Count));
        writableData.InsertRange(0, BitConverter.GetBytes(value));
        return writableData.ToArray();
    }
    
    public byte GetByte()
    {
        byte value = data[readPos];
        readPos ++;
        return value;
    }
    public byte[] GetBytes(int length)
    {
        byte[] value = new byte[length];
        Array.Copy(value, readPos, value, 0, length);
        readPos += length;
        return value;
    }

    public int GetInt()
    {
        int value = BitConverter.ToInt32(data, readPos);
        readPos += 4;
        return value;
    }
    public string GetString()
    {
        int length = GetInt();
        string value = Encoding.ASCII.GetString(data, readPos, length);
        readPos += length;
        return value;
    }

    public float GetFloat()
    {
        float value = BitConverter.ToSingle(data, readPos);
        readPos += 4;
        return value;
    }
    public Vector3 GetVector3()
    {
        return new Vector3(GetFloat(), GetFloat(), GetFloat());
    }
    public Quaternion GetQuaternion()
    {
        return new Quaternion(GetFloat(), GetFloat(), GetFloat(), GetFloat());
    }
    
    public void Add(byte data)
    {
        writableData.Add(data);
    }    
    public void Add(byte[] data)
    {
        writableData.AddRange(data);
    }
    public void Add(int data)
    {
        writableData.AddRange(BitConverter.GetBytes(data));
    }

    public void Add(float data)
    {
        writableData.AddRange(BitConverter.GetBytes(data));
    }

    public void Add(bool data)
    {
        writableData.AddRange(BitConverter.GetBytes(data));
    }
    
    public void Add(String data)
    {
        Add(data.Length);
        writableData.AddRange(Encoding.ASCII.GetBytes(data));
    }

    public void Add(Vector3 data)
    {
        Add(data.x);
        Add(data.y);
        Add(data.z);
    }

    public void Add(Quaternion data)
    {
        Add(data.x);
        Add(data.y);
        Add(data.z);
        Add(data.w);
    }
    
    public enum PacketID
    {
        S_welcome = 1,
        S_spawnPlayer = 2,
        S_playerPosition = 3,
        S_playerRotation = 4,
        S_playerShoot = 5,
        S_playerDisconnected = 6,
        S_playerHealth = 7,
        S_playerDead = 8,
        S_playerRespawned = 9,


        C_spawnPlayer = 126,
        C_welcomeReceived = 127,
        C_playerMovement = 128,
        C_playerShoot = 129,
        C_playerHit = 130,
    }
}