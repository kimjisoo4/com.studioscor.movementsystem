using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public interface IGravityModifier : IMovementModifier
    {
        public void SetGravity(float newGravity);
    }

    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Gravity Modifier", order: 10)]
    public class GravityModifierComponent : MovementModifierComponent, IGravityModifier
    {
        [Header(" [ Gravity Movement ] ")]
        [SerializeField] private float _Gravity = 9.81f;

        protected override void Reset()
        {
            base.Reset();

            _UpdateType = EMovementUpdateType.Late;
        }

        public void SetGravity(float newGravity)
        {
            _Gravity = newGravity;
        }

        protected override void UpdateMovement(float deltaTime)
        {
            if (MovementSystem.IsGrounded)
                return;

            float gravity = MovementSystem.PrevGravity;

            if (gravity.IsPositive())
                gravity = 0f;

            gravity -= _Gravity * deltaTime;

            MovementSystem.AddVelocity(Vector3.up * gravity);
        }
    }

    public class GravityModifier : MovementModifier, IGravityModifier
    {
        [Header(" [ Gravity Movement ] ")]
        [SerializeField] private float _Gravity = 9.81f;

        public GravityModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem) : base(movementSystem, moduleSystem)
        {
        }

        public void SetGravity(float newGravity)
        {
            _Gravity = newGravity;
        }

        protected override void UpdateMovement(float deltaTime)
        {
            if (_MovementSystem.IsGrounded)
                return;

            float gravity = _MovementSystem.PrevGravity;

            if (gravity.IsPositive())
                gravity = 0f;

            gravity -= _Gravity * deltaTime;

            _MovementSystem.AddVelocity(Vector3.up * gravity);
        }
    }

}