using UnityEngine;
namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Simple Directional Modifier", order: 5)]
    public class SimpleDirectionalModifier : MovementModifier, IMoveSpeedModifier
    {
        [Header(" [ Simple Diretional Movement ] ")]
        [SerializeField] private float _MaxSpeed = 5f;

        private float _CurrentSpeed;
        public float MaxSpeed => _MaxSpeed;
        public float CurrentSpeed => _CurrentSpeed;

        public void SetCurrentSpeed(float newSpeed)
        {
            _CurrentSpeed = newSpeed;
        }
        public void SetMaxSpeed(float newSpeed)
        {
            _MaxSpeed = newSpeed;
        }

        public override void ProcessMovement(float deltaTime)
        {
            _CurrentSpeed = _MaxSpeed * MovementSystem.MoveStrength;

            MovementSystem.AddVelocity(_CurrentSpeed * MovementSystem.MoveDirection);
        }
    }
}