using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public interface IGravityModifier : IMovementModifier
    {
        public void SetGravity(float newGravity);
    }

    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Gravity Modifier", order: 10)]
    public class GravityModifier : MovementModifier, IGravityModifier
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

        public override void ProcessMovement(float deltaTime)
        {
            if (MovementSystem.IsGrounded || MovementSystem.WasGrounded)
                return;

            float gravity = MovementSystem.PrevGravity;

            if (gravity.IsPositive())
                gravity = 0f;

            gravity -= _Gravity * deltaTime;

            MovementSystem.AddVelocity(Vector3.up * gravity);
        }

        
    }

}