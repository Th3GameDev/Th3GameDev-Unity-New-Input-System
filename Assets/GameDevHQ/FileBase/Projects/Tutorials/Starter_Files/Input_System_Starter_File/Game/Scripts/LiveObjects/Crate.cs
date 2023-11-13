using Game.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

namespace Game.Scripts.LiveObjects
{
    public class Crate : MonoBehaviour
    {
        [SerializeField] private float _punchDelay;
        [SerializeField] private float _holdPunchDelay = 1.5f;
        [SerializeField] private GameObject _wholeCrate, _brokenCrate;
        [SerializeField] private Rigidbody[] _pieces;
        [SerializeField] private BoxCollider _crateCollider;
        [SerializeField] private InteractableZone _interactableZone;
        private bool _isReadyToBreak = false;
        private bool _isBreaking = false;

        private bool _didPress = false;
        private bool _didRelease = false;

        private List<Rigidbody> _brakeOff = new List<Rigidbody>();

        private void OnEnable()
        {
            InteractableZone.onZoneInteractionComplete += InteractableZone_onZoneInteractionComplete;
            InputManager.Instance.OnInteractionEvent += HandleInteractionEvent;
        }

        private void InteractableZone_onZoneInteractionComplete(InteractableZone zone)
        {
            if (_isReadyToBreak == false && _brakeOff.Count > 0 && zone.GetZoneID() == 6)
            {
                _wholeCrate.SetActive(false);
                _brokenCrate.SetActive(true);
                _isReadyToBreak = true;
            }
        }

        private void Update()
        {
            _didRelease = InputManager.Instance.GetHoldReleaseInput();

            if (_isReadyToBreak && _interactableZone.GetZoneID() == 6) //Crate zone
            {
                if (_didPress && _brakeOff.Count > 0)
                {
                    UIManager.Instance.DisplayInteractableZoneMessage(false);
                    BreakPart();
                    StartCoroutine(PunchDelay());
                }
                else if (_didRelease && _brakeOff.Count > 0 && !_isBreaking)
                {
                    UIManager.Instance.DisplayInteractableZoneMessage(false);
                    _isBreaking = true;
                    StartCoroutine(BreakPiecesOnRelease());

                }
                else if (_brakeOff.Count == 0)
                {
                    _isReadyToBreak = false;
                    _crateCollider.enabled = false;
                    _interactableZone.CompleteTask(6);
                }
            }

            _didPress = false;
        }

        private void HandleInteractionEvent(bool tap, bool hold)
        {
            _didPress = tap;
        }

        private void Start()
        {
            _brakeOff.AddRange(_pieces);
        }

        public void BreakPart()
        {
            int rng = Random.Range(0, _brakeOff.Count);
            _brakeOff[rng].constraints = RigidbodyConstraints.None;
            _brakeOff[rng].AddForce(new Vector3(1f, 1f, 1f), ForceMode.Force);
            _brakeOff.Remove(_brakeOff[rng]);
        }

        private IEnumerator BreakPiecesOnRelease()
        {
            for (int i = 0; i < 3 && _brakeOff.Count > 0; i++)
            {
                BreakPart();
            }

            yield return new WaitForSeconds(_holdPunchDelay);
            _isBreaking = false;
            _interactableZone.ResetAction(6);
        }

        IEnumerator PunchDelay()
        {
            float delayTimer = 0;
            while (delayTimer < _punchDelay)
            {
                yield return new WaitForEndOfFrame();
                delayTimer += Time.deltaTime;
            }

            _interactableZone.ResetAction(6);
        }

        private void OnDisable()
        {
            InteractableZone.onZoneInteractionComplete -= InteractableZone_onZoneInteractionComplete;
            InputManager.Instance.OnInteractionEvent -= HandleInteractionEvent;
        }
    }
}
