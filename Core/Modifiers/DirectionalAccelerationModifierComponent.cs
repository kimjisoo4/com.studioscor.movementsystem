using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public interface IDirectionalModifier : IMovementModifier
    {
        public float MaxSpeed { get; }
        public float CurrentSpeed { get; }
        public void SetCurrentSpeed(float newSpeed);
        public void SetMaxSpeed(float newSpeed);
    }

    public abstract class MovementModifier : BaseClass, IMovementModifier
    {
        [Header(" [ Movement Modifier ] ")]
        [SerializeField] protected EMovementUpdateType updateType = EMovementUpdateType.Default;
        protected readonly IMovementSystem movementSystem;
        protected readonly IMovementModuleSystem moduleSystem;

        private bool isPlaying = false;
        public EMovementUpdateType UpdateType => updateType;
        public bool IsPlaying => isPlaying;

#if UNITY_EDITOR
        protected override Object Context => movementSystem.gameObject;
#endif

        public MovementModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem)
        {
            this.movementSystem = movementSystem;
            this.moduleSystem = moduleSystem;

            moduleSystem.AddModifier(this);
        }

        public void EnableModifier()
        {
            isPlaying = true;

            ResetModifier();
        }
        public void DisableModifier()
        {
            isPlaying = false;

            ResetModifier();
        }

        public void ProcessMovement(float deltaTime)
        {
            if (!isPlaying)
                return;

            UpdateMovement(deltaTime);
        }

        protected abstract void UpdateMovement(float deltaTime);
        public virtual void ResetModifier() { }
    }

    [System.Serializable]
    public class DirectionalAccelerationModifier : MovementModifier
    {
        [Header(" [ Directional Acceeration Modifier ] ")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float accelateSpeed = 10f;
        [SerializeField] private float decelerateSpeed = 20f;

        private float currentSpeed;
        public float MaxSpeed => maxSpeed;
        public float CurrentSpeed => currentSpeed;

        public DirectionalAccelerationModifier(IMovementSystem movementSystem, IMovementModuleSystem movementModuleSystem) : base(movementSystem, movementModuleSystem)
        {
            updateType = EMovementUpdateType.Default;
        }

        public void SetMaxSpeed(float newSpeed)
        {
            maxSpeed = newSpeed;
        }
        public void SetCurrentSpeed(float newSpeed)
        {
            currentSpeed = newSpeed;
        }
        public override void ResetModifier()
        {
            currentSpeed = 0f;
        }
        protected override void UpdateMovement(float deltaTime)
        {
            currentSpeed = movementSystem.PrevSpeed;
            float strength = movementSystem.MoveStrength;

            Vector3 prevVelocity = movementSystem.PrevVelocityXZ;
            Vector3 direction = movementSystem.MoveDirection;

            Log("PrevSpeed... ( Speed : " + currentSpeed.ToString("N1") + " )");
            Log("PrevVelocity... ( Prev VelocityXZ : " + prevVelocity.ToString() + " )");

            if (movementSystem.MoveStrength > 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed * strength, deltaTime * accelateSpeed);

                Log("Acceleate... ( Speed : " + currentSpeed.ToString("N1") + " )");
            }
            else
            {
                direction = movementSystem.PrevVelocityXZ.normalized;

                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deltaTime * decelerateSpeed);

                Log("Decelerate... ( Speed : " + currentSpeed.ToString("N1") + " )");
            }

            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            Log("Result... ( Speed : " + currentSpeed.ToString("N1") + " )");

            Vector3 velocity = prevVelocity + (direction * currentSpeed);

            velocity = Vector3.ClampMagnitude(velocity, currentSpeed);

            movementSystem.AddVelocity(velocity);
        }
    }

    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Directional Acceleration Modifier", order: 0)]
    public class DirectionalAccelerationModifierComponent : MovementModifierComponent, IDirectionalModifier
    {
        [Header(" [ Directional Acceeration Modifier ] ")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float accelateSpeed = 10f;
        [SerializeField] private float decelerateSpeed = 20f;

        private float currentSpeed;
        public float MaxSpeed => maxSpeed;
        public float CurrentSpeed => currentSpeed;

        public void SetMaxSpeed(float newSpeed)
        {
            maxSpeed = newSpeed;
        }
        public void SetCurrentSpeed(float newSpeed)
        {
            currentSpeed = newSpeed;
        }
        public override void ResetModifier()
        {
            currentSpeed = 0f;
        }
        protected override void UpdateMovement(float deltaTime)
        {
            currentSpeed = MovementSystem.PrevSpeed;
            float strength = MovementSystem.MoveStrength;

            Vector3 prevVelocity = MovementSystem.PrevVelocityXZ;
            Vector3 direction = MovementSystem.MoveDirection;

            Log("PrevSpeed... ( Speed : " + currentSpeed.ToString("N1") + " )");
            Log("PrevVelocity... ( Prev VelocityXZ : " + prevVelocity.ToString() + " )");

            if (MovementSystem.MoveStrength > 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed * strength, deltaTime * accelateSpeed);

                Log("Acceleate... ( Speed : " + currentSpeed.ToString("N1") + " )");
            }
            else
            {
                direction = MovementSystem.PrevVelocityXZ.normalized;

                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deltaTime * decelerateSpeed);

                Log("Decelerate... ( Speed : " + currentSpeed.ToString("N1") + " )");
            }

            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            Log("Result... ( Speed : " + currentSpeed.ToString("N1") + " )");

            Vector3 velocity = prevVelocity + (direction * currentSpeed);

            velocity = Vector3.ClampMagnitude(velocity, currentSpeed);

            MovementSystem.AddVelocity(velocity);
        }
    }

}