using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Game.Scripts.UI;

namespace Game.Scripts.LiveObjects
{
    public class Drone : MonoBehaviour
    {
        private enum Tilt
        {
            NoTilt, Forward, Back, Left, Right
        }

        [SerializeField]
        private Rigidbody _rigidbody;
        [SerializeField]
        private float _speed = 5f;
        private bool _inFlightMode = false;
        private bool _escKeyPressed = false;
        [SerializeField]
        private Animator _propAnim;
        [SerializeField]
        private CinemachineVirtualCamera _droneCam;
        [SerializeField]
        private InteractableZone _interactableZone;

        public static event Action OnEnterFlightMode;
        public static event Action onExitFlightmode;

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += EnterFlightMode;
        }

        private void EnterFlightMode(InteractableZone zone)
        {
            if (_inFlightMode != true && zone.GetZoneID() == 4) // drone Scene
            {
                _propAnim.SetTrigger("StartProps");
                _droneCam.Priority = 11;
                _inFlightMode = true;
                InputManager.Instance.EnableDroneActionMap();
                OnEnterFlightMode?.Invoke();
                UIManager.Instance.DroneView(true);
                _interactableZone.CompleteTask(4);
            }
        }

        private void ExitFlightMode()
        {
            _droneCam.Priority = 9;
            _inFlightMode = false;
            InputManager.Instance.DisableDroneActionMap();
            UIManager.Instance.DroneView(false);
        }

        private void Update()
        {
            _escKeyPressed = InputManager.Instance.GetEscapeKeyInput();

            if (_inFlightMode)
            {
                CalculateTilt();
                CalculateMovementUpdate();

                if (_escKeyPressed)
                {
                    _inFlightMode = false;
                    onExitFlightmode?.Invoke();
                    ExitFlightMode();
                }
            }
        }

        private void FixedUpdate()
        {
            _rigidbody.AddForce(transform.up * (9.81f), ForceMode.Acceleration);
            if (_inFlightMode)
                CalculateMovementFixedUpdate();
        }

        private void CalculateMovementUpdate()
        {
            float rotationInputX = InputManager.Instance.GetDroneRotationInput().x;

            if (rotationInputX < 0)
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y -= _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
            if (rotationInputX > 0)
            {
                var tempRot = transform.localRotation.eulerAngles;
                tempRot.y += _speed / 3;
                transform.localRotation = Quaternion.Euler(tempRot);
            }
        }

        private void CalculateMovementFixedUpdate()
        {
            float movementInputY = InputManager.Instance.GetDroneMovementInput().y;

            if (movementInputY > 0)
            {
                _rigidbody.AddForce(transform.up * _speed, ForceMode.Acceleration);
            }
            if (movementInputY < 0)
            {
                _rigidbody.AddForce(-transform.up * _speed, ForceMode.Acceleration);
            }
        }

        private void CalculateTilt()
        {
            float tiltInputX = InputManager.Instance.GetDroneMovementInput().x;
            float tiltInputZ = InputManager.Instance.GetDroneMovementInput().z;

            if (tiltInputX > 0)
                transform.rotation = Quaternion.Euler(00, transform.localRotation.eulerAngles.y, 30);
            else if (tiltInputX < 0)
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, -30);
            else if (tiltInputZ > 0)
                transform.rotation = Quaternion.Euler(30, transform.localRotation.eulerAngles.y, 0);
            else if (tiltInputZ < 0)
                transform.rotation = Quaternion.Euler(-30, transform.localRotation.eulerAngles.y, 0);
            else
                transform.rotation = Quaternion.Euler(0, transform.localRotation.eulerAngles.y, 0);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= EnterFlightMode;
        }
    }
}
