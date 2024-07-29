using StudioScor.Utilities;
using UnityEngine;

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
        [SerializeField] private GravityModifier _modifier;

        protected override void Reset()
        {
            base.Reset();

            _updateType = EMovementUpdateType.Late;
        }

        public void SetGravity(float newGravity)
        {
            _modifier.SetGravity(newGravity);
        }

        protected override void UpdateMovement(float deltaTime)
        {
            _modifier.ProcessMovement(deltaTime);
        }
    }

    public class GravityModifier : MovementModifier, IGravityModifier
    {
        [Header(" [ Gravity Movement ] ")]
        [SerializeField] private float _Gravity = 9.81f;

        public GravityModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem, EMovementUpdateType updateType = EMovementUpdateType.Default) : base(movementSystem, moduleSystem, updateType)
        {
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

}