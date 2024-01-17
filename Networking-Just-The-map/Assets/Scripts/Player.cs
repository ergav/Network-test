using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public string userName;
    public Vector3 targetPosition;
    public CharacterController characterController;

    public virtual void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public virtual void Update()
    {
        characterController.Move(targetPosition * Time.deltaTime);
    }
}