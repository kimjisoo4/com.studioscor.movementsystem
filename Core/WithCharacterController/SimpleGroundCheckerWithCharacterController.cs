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
                _normal = Vector3.up;
                _distance = 0f;
                _point = _CharacterController.transform.position;
            }
            else
            {
                _normal = Vector3.up;
                _distance = default;
                _point = default;
            }

            SetGrounded(ground);
        }
    }

}