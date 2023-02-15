using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/GroundChecker/Ground Checker With Character Controller", order: 0)]
    public class GroundCheckerWithCharacterController : GroundChecker
    {
        [Header(" [ With Character Controller ] ")]
        [SerializeField] private CharacterController _CharacterController;
        [SerializeField, Range(0.01f, 1f)] private float _RadiusScale = 1f;
        [SerializeField] private float _StairOffset = 0.25f;
        [SerializeField] private float _AirOffset = 0.05f;

        private float Offset => IsGrounded ? _StairOffset : _AirOffset;
        
        private void OnDrawGizmosSelected()
        {
            if (!UseDebug)
                return;

            Vector3 start = _CharacterController.bounds.center;
            Vector3 direction = -transform.up;

            float radius = _CharacterController.radius * _RadiusScale;
            float height = (_CharacterController.height * 0.5f) + _CharacterController.skinWidth;
            float distance = height - radius;


            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(start + direction * (distance + _StairOffset), radius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(start + direction * (distance + _AirOffset), radius);
        }

        protected override void OnSetup()
        {
            base.OnSetup();

            if (!_CharacterController)
            {
                if(!gameObject.TryGetComponentInParentOrChildren(out _CharacterController))
                {
                    Log("Character Contollre Is NULL", true);
                }
            }
        }

        public override void CheckGrounded()
        {
            Vector3 start = _CharacterController.bounds.center;
            Vector3 direction = -transform.up;

            float radius = _CharacterController.radius * _RadiusScale;
            float height = (_CharacterController.height * 0.5f) + _CharacterController.skinWidth;
            float distance = height - radius;

            bool isHit = SUtility.Physics.DrawSphereCast(start, radius, direction, distance + Offset, out RaycastHit hit, GroundLayer, UseDebug);

            SetGrounded(isHit);

            if (isHit)
            {
                _Normal = hit.normal;
                _Distance = hit.distance - distance;
                _Point = hit.point;

                if (_Distance < 0.01f)
                {
                    _Distance = 0f;
                }
            }
            else
            {
                _Normal = Vector3.up;
                _Distance = default;
                _Point = default;
            }
        }
    }

}