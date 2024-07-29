using UnityEngine;

namespace StudioScor.MovementSystem
{

    public interface IAccelerationModifier : IDirectionalModifier
    {
        public void SetAccelateSpeed(float newSpeed);
        public void SetDecelerateSpeed(float newSpeed);
    }

    [System.Serializable]
    public class DirectionalAccelerationModifier : MovementModifier, IAccelerationModifier
    {
        [Header(" [ Directional Acceeration Modifier ] ")]
        [SerializeField] private float maxSpeed = 5f;
        [SerializeField] private float accelateSpeed = 10f;
        [SerializeField] private float decelerateSpeed = 20f;

        private float currentSpeed;

        public DirectionalAccelerationModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem, EMovementUpdateType updateType = EMovementUpdateType.Default) : base(movementSystem, moduleSystem, updateType)
        {
        }

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

    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Directional Acceleration Modifier", order: 0)]
    public class DirectionalAccelerationModifierComponent : MovementModifierComponent, IDirectionalModifier, IAccelerationModifier
    {
        [Header(" [ Directional Acceeration Modifier ] ")]
        [SerializeField] private DirectionalAccelerationModifier _modifier;
        public float MaxSpeed => _modifier.MaxSpeed;
        public float CurrentSpeed => _modifier.CurrentSpeed;

        public override void EnableModifier()
        {
            base.EnableModifier();

            _modifier.SetupModifier(MovementSystem, MovementModuleSystem, UpdateType);
        }
        public void SetMaxSpeed(float newSpeed)
        {
            _modifier.SetMaxSpeed(newSpeed);
        }
        public void SetCurrentSpeed(float newSpeed)
        {
            _modifier.SetCurrentSpeed(newSpeed);
        }
        public override void ResetModifier()
        {
            _modifier.ResetModifier();
        }
        protected override void UpdateMovement(float deltaTime)
        {
            _modifier.ProcessMovement(deltaTime);
        }
        public void SetAccelateSpeed(float newSpeed)
        {
            _modifier.SetAccelateSpeed(newSpeed);
        }
        public void SetDecelerateSpeed(float newSpeed)
        {
            _modifier.SetDecelerateSpeed(newSpeed);
        }
    }
}