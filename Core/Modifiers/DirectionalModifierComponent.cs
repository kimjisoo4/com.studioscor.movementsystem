using UnityEngine;
namespace StudioScor.MovementSystem
{
    public interface IDirectionalModifier : IMovementModifier
    {
        public float MaxSpeed { get; }
        public float CurrentSpeed { get; }
        public void SetCurrentSpeed(float newSpeed);
        public void SetMaxSpeed(float newSpeed);
    }
    public class DirectionalModifier : MovementModifier, IDirectionalModifier
    {
        [Header(" [ Simple Diretional Movement ] ")]
        [SerializeField] private float maxSpeed = 5f;

        private float currentSpeed;

        public DirectionalModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem, EMovementUpdateType updateType = EMovementUpdateType.Default) : base(movementSystem, moduleSystem, updateType)
        {
        }

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
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Simple Directional Modifier", order: 5)]
    public class DirectionalModifierComponent : MovementModifierComponent, IDirectionalModifier
    {
        [Header(" [ Simple Diretional Movement ] ")]
        [SerializeField] private DirectionalModifier _modifier;
        public float MaxSpeed => _modifier.MaxSpeed;
        public float CurrentSpeed => _modifier.CurrentSpeed;

        public void SetCurrentSpeed(float newSpeed)
        {
            _modifier.SetCurrentSpeed(newSpeed);
        }
        public void SetMaxSpeed(float newSpeed)
        {
            _modifier.SetMaxSpeed(newSpeed);
        }

        protected override void UpdateMovement(float deltaTime)
        {
            _modifier.ProcessMovement(deltaTime);
        }
    }
}