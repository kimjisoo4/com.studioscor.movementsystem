using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/GroundChecker/Ground Checker With Character Controller", order: 0)]
    public class GroundCheckerWithCharacterController : GroundChecker
    {
        [Header(" [ With Character Controller ] ")]
        [SerializeField] private CharacterController characterController;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField, Range(0.01f, 1f)] private float radiusScale = 1f;
        [SerializeField] private float stairOffset = 0.25f;
        [SerializeField] private float airOffset = 0.05f;

        protected LayerMask GroundLayer => groundLayer;
        private float Offset => IsGrounded ? stairOffset : airOffset;
        
        private void OnDrawGizmosSelected()
        {
            if (!UseDebug)
                return;

            Vector3 start = characterController.bounds.center;
            Vector3 direction = -transform.up;

            float radius = characterController.radius * radiusScale;
            float height = (characterController.height * 0.5f) + characterController.skinWidth;
            float distance = height - radius;


            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(start + direction * (distance + stairOffset), radius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(start + direction * (distance + airOffset), radius);
        }

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
            if (!characterController)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out characterController))
                {
                    LogError($"{nameof(characterController)} is NULL!!");
                }
            }
        }

        public override void CheckGrounded()
        {
            Vector3 start = characterController.bounds.center;
            Vector3 direction = -transform.up;

            float radius = characterController.radius * radiusScale;
            float height = (characterController.height * 0.5f) + characterController.skinWidth;
            float distance = height - radius;

            bool isHit = SUtility.Physics.DrawSphereCast(start, radius, direction, distance + Offset, out RaycastHit hit, GroundLayer, UseDebug);


            if (isHit)
            {
                _normal = hit.normal;
                _distance = hit.distance - distance;
                _point = hit.point;

                if (_distance < 0.01f)
                {
                    _distance = 0f;
                }
            }
            else
            {
                _normal = Vector3.up;
                _distance = default;
                _point = default;
            }

            SetGrounded(isHit);
        }
    }

}