using UnityEngine;

namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Stair Modifier", order: 20)]
    public class StairModifier : Modifier
    {
        public override void ProcessMovement(float deltaTime)
        {
            if (MovementSystem.IsGrounded && MovementSystem.WasGrounded)
            {
                MovementSystem.MovePosition(transform.up * -MovementSystem.GroundDistance);
            }
        }
    }

}