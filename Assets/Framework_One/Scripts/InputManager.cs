using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    public static InputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log("Input Manager is NULL.");
            }

            return _instance;
        }
    }

    private GameInputActions _input;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _input = new GameInputActions();
        _input.Player.Enable();
    }

 
    void Update()
    {
       
    }

    public Vector3 GetMovementInput()
    {
        return _input.Player.Movement.ReadValue<Vector3>();
    }
}
