using UnityEngine;

namespace StudioScor.MovementSystem
{
    public interface IStairModifier : IMovementModifier
    {

    }

    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Stair Modifier", order: 20)]
    public class StairModifierComponent : MovementModifierComponent, IStairModifier
    {
        private bool _WasGrounded = false;

        protected override void UpdateMovement(float deltaTime)
        {
            if (MovementSystem.IsGrounded && _WasGrounded)
                MovementSystem.MovePosition(transform.up * -MovementSystem.GroundDistance);

            _WasGrounded = MovementSystem.IsGrounded;
        }
    }

    public class StairModifier : MovementModifier, IStairModifier
    {
        private bool _WasGrounded = false;
        private Transform transform => _MovementSystem.transform;
        public StairModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem) : base(movementSystem, moduleSystem)
        {
        }


        protected override void UpdateMovement(float deltaTime)
        {
            if (_MovementSystem.IsGrounded && _WasGrounded)
            {
                float distance = _MovementSystem.GroundDistance * -1;

                _MovementSystem.MovePosition(transform.up * distance);
            }

            _WasGrounded = _MovementSystem.IsGrounded;
        }
    }

}