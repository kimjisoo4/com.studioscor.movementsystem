using UnityEngine;
using StudioScor.Utilities;
using UnityEngine.UIElements;

namespace StudioScor.MovementSystem
{
    public class MovementSystemWithRigidbody : MovementSystemComponent
    {
        [Header(" [ Use Rigidbody ] ")]
        [SerializeField] private Rigidbody _rigidbody;

        private bool _wasRequestTeleport;
        private Vector3 _requestTeleportPosition;
        private Vector3 _lastVelocity;
        public override Vector3 LastVelocity => _lastVelocity;
       

        private void Reset()
        {
            gameObject.TryGetComponentInParentOrChildren(out _rigidbody);
        }
        protected override void OnSetup()
        {
            base.OnSetup();

            if (!_rigidbody)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _rigidbody))
                {
                    LogError("Rigidbody Is NULL");
                }
            }
        }
        public override void Teleport(Vector3 position = default, bool isImmediately = true)
        {
            if(isImmediately)
            {
                transform.position = position;
                _rigidbody.position = position;
            }
            else
            {
                _wasRequestTeleport = true;
                _requestTeleportPosition = position;
            }
        }
        private void OnTeleport()
        {
            transform.position = _requestTeleportPosition;
            _rigidbody.position = _requestTeleportPosition;
        }

        protected override void OnMovement(float deltaTime)
        {
            if (_rigidbody.isKinematic)
            {
                _lastVelocity = _addVelocity * deltaTime;

                if (_addPosition != default)
                {
                    _lastVelocity += _addPosition;
                }

                _rigidbody.MovePosition(_rigidbody.position + LastVelocity);
            }
            else
            {
                _lastVelocity = _addVelocity;

                if (_addPosition != default)
                {
                    _lastVelocity += _addPosition.SafeDivide(deltaTime);
                }

                _rigidbody.linearVelocity = LastVelocity;
            }

            if (_wasRequestTeleport)
            {
                OnTeleport();
            }
        }
    }

}