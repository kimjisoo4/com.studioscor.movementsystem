using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{

    [AddComponentMenu("StudioScor/MovementSystem/Movement System With CharacterController", order:0)]
    public class MovementSystemWithCharacterController : MovementSystemComponent
    {
        [Header(" [ Use Character Controller ] ")]
        [SerializeField] private CharacterController _CharacterController;

        private Vector3 _LastVelocity;
        protected override Vector3 LastVelocity => _LastVelocity;

        private void Reset()
        {
            gameObject.TryGetComponentInParentOrChildren(out _CharacterController);
        }

        protected override void OnSetup()
        {
            base.OnSetup();

            if (!_CharacterController)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _CharacterController))
                {
                    Log("Character Contollre Is NULL", true);
                }
            }
        }

        protected override void OnMovement(float deltaTime)
        {
            _LastVelocity = _AddVelocity;

            if(_AddPosition != default)
            {
                _LastVelocity += _AddPosition / deltaTime;
            }

            _CharacterController.Move(_LastVelocity * deltaTime);
        }
    }

}