using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public class MovementSystemWithRigidbody : MovementSystemComponent
    {
        [Header(" [ Use Rigidbody ] ")]
        [SerializeField] private Rigidbody _rigidbody;

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
        public override void Teleport(Vector3 position)
        {
            transform.position = position;
            _rigidbody.position = position;
        }

        protected override void OnMovement(float deltaTime)
        {
            if (_rigidbody.isKinematic)
            {
                _lastVelocity = addVelocity * deltaTime;

                if(addPosition != default)
                {
                    _lastVelocity += addPosition;
                }

                _rigidbody.MovePosition(_rigidbody.position + LastVelocity);
            }
            else
            {
                _lastVelocity = addVelocity;

                if (addPosition != default)
                {
                    _lastVelocity += addPosition.SafeDivide(deltaTime);
                }

                _rigidbody.velocity = LastVelocity;
            }
            
        }
    }

}