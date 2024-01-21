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
    public interface IAccelerationModifier : IDirectionalModifier
    {
        public void SetAccelateSpeed(float newSpeed);
        public void SetDecelerateSpeed(float newSpeed);
    }

    public abstract class MovementModifier : BaseClass, IMovementModifier
    {
        [Header(" [ Movement Modifier ] ")]
        [SerializeField] protected EMovementUpdateType _UpdateType = EMovementUpdateType.Default;
        protected readonly IMovementSystem _MovementSystem;
        protected readonly IMovementModuleSystem _ModuleSystem;

        private bool _IsPlaying = false;
        public EMovementUpdateType UpdateType => _UpdateType;
        public bool IsPlaying => _IsPlaying;

#if UNITY_EDITOR
        public override Object Context => _MovementSystem.gameObject;
#endif

        public MovementModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem)
        {
            this._MovementSystem = movementSystem;
            this._ModuleSystem = moduleSystem;

            moduleSystem.AddModifier(this);
        }

        public void EnableModifier()
        {
            _IsPlaying = true;

            ResetModifier();
        }
        public void DisableModifier()
        {
            _IsPlaying = false;

            ResetModifier();
        }

        public void ProcessMovement(float deltaTime)
        {
            if (!_IsPlaying)
                return;

            UpdateMovement(deltaTime);
        }

        protected abstract void UpdateMovement(float deltaTime);
        public virtual void ResetModifier() { }
    }

    [System.Serializable]
    public class DirectionalAccelerationModifier : MovementModifier, IAccelerationModifier
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
            _UpdateType = EMovementUpdateType.Default;
        }

        public void SetMaxSpeed(float newSpeed)
        {
            maxSpeed = newSpeed;
        }
        public void SetCurrentSpeed(float newSpeed)
        {
            currentSpeed = newSpeed;
        }
        public void SetAccelateSpeed(float newSpeed)
        {
            accelateSpeed = newSpeed;
        }
        public void SetDecelerateSpeed(float newSpeed)
        {
            decelerateSpeed = newSpeed;
        }
        public override void ResetModifier()
        {
            currentSpeed = 0f;
        }
        protected override void UpdateMovement(float deltaTime)
        {
            currentSpeed = _MovementSystem.PrevSpeed;
            float strength = _MovementSystem.MoveStrength;

            Vector3 prevVelocity = _MovementSystem.PrevVelocityXZ;
            Vector3 direction = _MovementSystem.MoveDirection;

            Log("PrevSpeed... ( Speed : " + currentSpeed.ToString("N1") + " )");
            Log("PrevVelocity... ( Prev VelocityXZ : " + prevVelocity.ToString() + " )");

            if (_MovementSystem.MoveStrength > 0)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed * strength, deltaTime * accelateSpeed);

                Log("Acceleate... ( Speed : " + currentSpeed.ToString("N1") + " )");
            }
            else
            {
                direction = _MovementSystem.PrevVelocityXZ.normalized;

                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deltaTime * decelerateSpeed);

                Log("Decelerate... ( Speed : " + currentSpeed.ToString("N1") + " )");
            }

            currentSpeed = Mathf.Min(currentSpeed, maxSpeed);

            Log("Result... ( Speed : " + currentSpeed.ToString("N1") + " )");

            Vector3 velocity = prevVelocity + (direction * currentSpeed);

            velocity = Vector3.ClampMagnitude(velocity, currentSpeed);

            _MovementSystem.AddVelocity(velocity);
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