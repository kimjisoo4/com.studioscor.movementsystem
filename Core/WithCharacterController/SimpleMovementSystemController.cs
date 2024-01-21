using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/Simple Movement System Component", order: 30)]
    public class SimpleMovementSystemController : BaseMonoBehaviour
    {
        [Header(" [ Movement System Controller ] ")]
        [SerializeField] private MovementSystemComponent _MovementSystem;
        [SerializeField] private GroundChecker _GroundChecker;

        [Header(" [ Setting ] ")]
        [SerializeField] private float _MoveSpeed = 5f;
        [SerializeField] private float _Gravity = 20f;
        [SerializeField] private float _Mass = 1f;
        [SerializeField] private float _Drag = 20f;

        [Header(" [ Modifiers ] ")]
        [SerializeField] private DirectionalAccelerationModifierComponent _DirectionalModifier;
        [SerializeField] private GravityModifierComponent _GravityModifier;
        [SerializeField] private ForceModifier _ForceModifier;

        private void Reset()
        {
            gameObject.TryGetComponentInParentOrChildren(out _GroundChecker);
            gameObject.TryGetComponentInParentOrChildren(out _MovementSystem);

            gameObject.TryGetComponentInParentOrChildren(out _DirectionalModifier);
            gameObject.TryGetComponentInParentOrChildren(out _GravityModifier);
            gameObject.TryGetComponentInParentOrChildren(out _ForceModifier);
        }

        private void Awake()
        {
            if (!_MovementSystem)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _MovementSystem))
                {
                    LogError("Movement System Is NULL!");
                }
            }
            if (!_GroundChecker)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _GroundChecker))
                {
                    LogError("Ground Checker Is NULL!");
                }
            }
            if (!_DirectionalModifier)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _DirectionalModifier))
                {
                    LogError("Directional Modifier Is NULL!");
                }
            }
            if (!_GravityModifier)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _GravityModifier))
                {
                    LogError("Gravity Modifier Is NULL!");
                }
            }
            if (!_ForceModifier)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _ForceModifier))
                {
                    LogError("Force Modifier Is NULL!");
                }
            }
        }

        private void Start()
        {
            SetMoveSpeed(_MoveSpeed);
            SetGravity(_Gravity);
            SetMass(_Mass);
            SetDrag(_Drag);
        }
        
        public void SetMoveDirection(Vector3 direction, float strength = -1f)
        {
            _MovementSystem.SetMoveDirection(direction, strength);
        }

        public void SetMoveSpeed(float newSpeed)
        {
            _DirectionalModifier.SetMaxSpeed(newSpeed);
        }
        public void SetGravity(float newGravity)
        {
            _GravityModifier.SetGravity(newGravity);
        }
        public void SetMass(float newMass)
        {
            _ForceModifier.SetMass(newMass);
        }
        public void SetDrag(float newDrag)
        {
            _ForceModifier.SetDrag(newDrag);
        }

        public void AddForce(Vector3 addForce, bool useOverride = false)
        {
            if(useOverride)
                _ForceModifier.OverrideForce(addForce);
            else
                _ForceModifier.AddForce(addForce);

            if (_ForceModifier.Force.y > 0f)
            {
                _MovementSystem.ForceUnGrounded();
            }
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;

            _GroundChecker.CheckGrounded();

            if (_ForceModifier.Force.y < 3f)
            {
                _MovementSystem.SetGrounded(_GroundChecker.IsGrounded);
                _MovementSystem.SetGroundState(_GroundChecker.Point, _GroundChecker.Normal, _GroundChecker.Distance);
            }

            _MovementSystem.UpdateMovement(deltaTime);
        }
    }

}