using UnityEngine;

namespace StudioScor.MovementSystem
{
    public interface IStairModifier : IMovementModifier
    {

    }

    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Stair Modifier", order: 20)]
    public class StairModifier : MovementModifierComponent, IStairModifier
    {
        protected override void UpdateMovement(float deltaTime)
        {
            if (MovementSystem.IsGrounded)
                MovementSystem.MovePosition(transform.up * -MovementSystem.GroundDistance);
        }
    }

}