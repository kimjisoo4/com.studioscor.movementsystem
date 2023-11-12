using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/GroundChecker/Simple Ground Checker With Character Controller", order: 0)]
    public class SimpleGroundCheckerWithCharacterController : GroundChecker
    {
        [Header(" [ With Character Controller ] ")]
        [SerializeField] private CharacterController _CharacterController;

        private void Reset()
        {
            SetReference();
        }
        protected override void OnSetup()
        {
            base.OnSetup();

            SetReference();
        }

        private void SetReference()
        {
            if (!_CharacterController)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _CharacterController))
                {
                    LogError($"{nameof(_CharacterController)} is NULL!!");
                }
            }
        }
        public override void CheckGrounded()
        {
            bool ground = _CharacterController.isGrounded || _CharacterController.collisionFlags.Equals(CollisionFlags.Below);

            if (ground)
            {
                _Normal = Vector3.up;
                _Distance = 0f;
                _Point = _CharacterController.transform.position;
            }
            else
            {
                _Normal = Vector3.up;
                _Distance = default;
                _Point = default;
            }

            SetGrounded(ground);
        }
    }

}