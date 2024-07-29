using UnityEngine;

namespace StudioScor.MovementSystem
{
    public interface IStairModifier : IMovementModifier
    {

    }

    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Stair Modifier", order: 20)]
    public class StairModifierComponent : MovementModifierComponent, IStairModifier
    {
        private StairModifier _modifier;

        protected override void UpdateMovement(float deltaTime)
        {
            _modifier.ProcessMovement(deltaTime);
        }
    }

    public class StairModifier : MovementModifier, IStairModifier
    {
        private bool _WasGrounded = false;
        private Transform transform => MovementSystem.transform;
        public StairModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem) : base(movementSystem, moduleSystem)
        {
        }


        protected override void UpdateMovement(float deltaTime)
        {
            if (MovementSystem.IsGrounded && _WasGrounded)
            {
                float distance = MovementSystem.GroundDistance * -1;

                MovementSystem.MovePosition(transform.up * distance);
            }

            _WasGrounded = MovementSystem.IsGrounded;
        }
    }

}