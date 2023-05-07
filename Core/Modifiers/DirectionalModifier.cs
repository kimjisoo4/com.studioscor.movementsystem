using UnityEngine;
namespace StudioScor.MovementSystem
{
    public interface IMoveSpeedModifier : IMovementModifier
    {
        public float MaxSpeed { get; }
        public float CurrentSpeed { get; }
        public void SetCurrentSpeed(float newSpeed);
        public void SetMaxSpeed(float newSpeed);
    }

    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Directional Modifier", order: 0)]
    public class DirectionalModifier : MovementModifier, IMoveSpeedModifier
    {
        [Header(" [ Diretional Movement ] ")]
        [SerializeField] private float _MaxSpeed = 5f;
        [SerializeField] private float _AccelateSpeed = 10f;
        [SerializeField] private float _DecelerateSpeed = 20f;

        private float _CurrentSpeed = 0f;

        public float MaxSpeed => _MaxSpeed;
        public float CurrentSpeed => _CurrentSpeed;

        public void SetMaxSpeed(float newSpeed)
        {
            _MaxSpeed = newSpeed;
        }
        public void SetCurrentSpeed(float newSpeed)
        {
            _CurrentSpeed = newSpeed;
        }

        public override void ProcessMovement(float deltaTime)
        {
            _CurrentSpeed = MovementSystem.PrevSpeed;
            float strength = MovementSystem.MoveStrength;

            Vector3 prevVelocity = MovementSystem.PrevVelocityXZ;
            Vector3 direction = MovementSystem.MoveDirection;

            Log("PrevSpeed... ( Speed : " + _CurrentSpeed.ToString("N1") + " )");
            Log("PrevVelocity... ( Prev VelocityXZ : " + prevVelocity.ToString() + " )");

            if (MovementSystem.MoveStrength > 0)
            {
                _CurrentSpeed = Mathf.MoveTowards(_CurrentSpeed, _MaxSpeed * strength, deltaTime * _AccelateSpeed);

                Log("Acceleate... ( Speed : " + _CurrentSpeed.ToString("N1")  + " )");
            }
            else
            {
                direction = MovementSystem.PrevVelocityXZ.normalized;

                _CurrentSpeed = Mathf.MoveTowards(_CurrentSpeed, 0f, deltaTime * _DecelerateSpeed);

                Log("Decelerate... ( Speed : " + _CurrentSpeed.ToString("N1") + " )");
            }

            _CurrentSpeed = Mathf.Min(_CurrentSpeed, _MaxSpeed);

            Log("Result... ( Speed : " + _CurrentSpeed.ToString("N1") + " )");

            Vector3 velocity = prevVelocity + (direction * _CurrentSpeed);

            velocity = Vector3.ClampMagnitude(velocity, _CurrentSpeed);
            
            MovementSystem.AddVelocity(velocity);
        }
    }

}