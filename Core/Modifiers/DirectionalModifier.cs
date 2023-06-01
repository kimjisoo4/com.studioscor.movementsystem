using UnityEngine;
namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Simple Directional Modifier", order: 5)]
    public class DirectionalModifier : MovementModifierComponent, IDirectionalModifier
    {
        [Header(" [ Simple Diretional Movement ] ")]
        [SerializeField] private float maxSpeed = 5f;

        private float currentSpeed;
        public float MaxSpeed => maxSpeed;
        public float CurrentSpeed => currentSpeed;

        public void SetCurrentSpeed(float newSpeed)
        {
            currentSpeed = newSpeed;
        }
        public void SetMaxSpeed(float newSpeed)
        {
            maxSpeed = newSpeed;
        }

        protected override void UpdateMovement(float deltaTime)
        {
            currentSpeed = maxSpeed * MovementSystem.MoveStrength;

            MovementSystem.AddVelocity(currentSpeed * MovementSystem.MoveDirection);
        }
    }
}