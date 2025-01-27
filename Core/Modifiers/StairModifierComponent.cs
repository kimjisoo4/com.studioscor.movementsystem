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

        protected override void Reset()
        {
            base.Reset();

            _updateType = EMovementUpdateType.Late;
        }

        protected override void UpdateMovement(float deltaTime)
        {
            _modifier.ProcessMovement(deltaTime);
        }
    }

    public class StairModifier : MovementModifier, IStairModifier
    {
        private bool _wasGrounded = false;

        public StairModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem, EMovementUpdateType updateType = EMovementUpdateType.Late) : base(movementSystem, moduleSystem, updateType)
        {
        }

        private Transform transform => MovementSystem.transform;
        

        protected override void UpdateMovement(float deltaTime)
        {
            if (MovementSystem.IsGrounded && _wasGrounded)
            {
                float distance = -MovementSystem.GroundDistance;

                MovementSystem.MovePosition(transform.up * distance);
            }

            _wasGrounded = MovementSystem.IsGrounded;
        }
    }

}