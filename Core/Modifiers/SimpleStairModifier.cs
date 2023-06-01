using UnityEngine;

namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Simple Stair Modifier", order: 25)]
    public class SimpleStairModifier : MovementModifierComponent
    {
        [Header(" [ Simple Stair Movement ] ")]
        [SerializeField] private float _Gravity = 9.81f;


        protected override void Reset()
        {
            base.Reset();

            _UpdateType = EMovementUpdateType.Late;
        }

        protected override void UpdateMovement(float deltaTime)
        {
            if (MovementSystem.IsGrounded)
                MovementSystem.AddVelocity(transform.up * -_Gravity);
        }
    }

}