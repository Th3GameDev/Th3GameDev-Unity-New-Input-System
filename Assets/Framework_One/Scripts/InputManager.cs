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

    public Action<bool, bool> OnInteractionEvent;

    private GameInputActions _input;

    private bool _hasTapped = false;
    private bool _isHolding = false;
    private bool _escPressed;

    private bool _canPress = false;

    private float _pressRate = 0.5f;
    private float _timeToNextpress;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        _input = new GameInputActions();
        _input.Player.Enable();

        _input.Player.Interactable.started += Interactable_started;
        _input.Player.Interactable.canceled += Interactable_canceled;
        _input.Player.Interactable.performed += Interactable_performed;

        _input.Player.ESC.started += ESC_started;
        _input.Player.ESC.performed += ESC_performed;
        _input.Player.ESC.canceled += ESC_canceled;

        _canPress = true;
    }

    private void ESC_started(InputAction.CallbackContext context)
    {
        _escPressed = true;
    }

    private void ESC_performed(InputAction.CallbackContext context)
    {
        _escPressed = false;
    }

    private void ESC_canceled(InputAction.CallbackContext context)
    {
        _escPressed = false;
    }

    private void Interactable_started(InputAction.CallbackContext context)
    {
        if (context.interaction is SlowTapInteraction)
        {
            _isHolding = true;
            _hasTapped = false;
        }
        else
        {
            _isHolding = false;
        }

        NotifyInteractionEvent();
    }

    private void Interactable_performed(InputAction.CallbackContext context)
    {
        if (_canPress)
        {
            if (context.interaction is SlowTapInteraction)
            {
                _isHolding = false;
                _hasTapped = false;
            }
            else
            {
                _canPress = false;
                _hasTapped = true;
            }
        }

        NotifyInteractionEvent();
    }

    private void Interactable_canceled(InputAction.CallbackContext context)
    {
        if (context.interaction is SlowTapInteraction)
        {
            _isHolding = false;
        }
        else
        {
            _hasTapped = false;
        }

        NotifyInteractionEvent();
    }


    void Update()
    {
        //Interactable Key Press Cooldown
        if (Time.time > _timeToNextpress)
        {
            _canPress = true;
            _timeToNextpress = Time.time + _pressRate;
        }

        _hasTapped = false;
    }

    public bool GetEscapeInput()
    {
        return _escPressed;
    }

    public Vector3 GetMovementInput()
    {
        return _input.Player.Movement.ReadValue<Vector3>();
    }

    public string GetKeyName()
    {
        return _input.Player.Interactable.controls[0].name.ToUpper();
    }

    private void NotifyInteractionEvent()
    {
        OnInteractionEvent?.Invoke(_hasTapped, _isHolding);
    }
}