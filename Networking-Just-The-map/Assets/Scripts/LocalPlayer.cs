using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalPlayer : Player
{
    public Transform cameraTransform;

    public override void Awake()
    {
        base.Awake();
        cameraTransform = Camera.main.transform;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public override void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X");
        float mouseY = Input.GetAxisRaw("Mouse Y");
        
        
        transform.Rotate(200 * mouseX * Time.deltaTime * Vector3.up);

        Vector3 rotation = cameraTransform.rotation.eulerAngles + new Vector3(-mouseY, 0, 0);
        rotation.x = ClampAngle(rotation.x, -90, 90);
        cameraTransform.eulerAngles = rotation;

        bool[] inputs = new bool[]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
        };
        Packet movePacket = new Packet();
        movePacket.Add((byte)Packet.PacketID.C_playerMovement);
        movePacket.Add(inputs.Length);
        foreach (var input in inputs)
        {
            movePacket.Add(input);
        }
        movePacket.Add(transform.rotation);
        Client.instance.udp.SendData(movePacket);
        
        base.Update();
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < 0)
        {
            angle = 360 + angle;
        }

        if (angle > 180)
        {
            return Mathf.Max(angle, 360 + min);
        }

        return Mathf.Min(angle, max);
    }
}