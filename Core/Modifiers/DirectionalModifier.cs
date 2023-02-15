using UnityEngine;
namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Directional Modifier", order: 0)]
    public class DirectionalModifier : Modifier
    {
        [Header(" [ Diretional Movement ] ")]
        [SerializeField] private float _MaxSpeed = 5f;
        [SerializeField] private float _AccelateSpeed = 10f;
        [SerializeField] private float _DecelerateSpeed = 20f;

        private float _Speed = 0f;

        public void SetMaxSpeed(float newSpeed)
        {
            _MaxSpeed = newSpeed;
        }

        public override void ProcessMovement(float deltaTime)
        {
            _Speed = MovementSystem.PrevSpeed;
            float strength = MovementSystem.MoveStrength;

            Vector3 prevVelocity = MovementSystem.PrevVelocityXZ;
            Vector3 direction = MovementSystem.MoveDirection;

            Log("PrevSpeed... ( Speed : " + _Speed.ToString("N1") + " )");
            Log("PrevVelocity... ( Prev VelocityXZ : " + prevVelocity.ToString() + " )");

            if (MovementSystem.MoveStrength > 0)
            {
                _Speed = Mathf.MoveTowards(_Speed, _MaxSpeed * strength, deltaTime * _AccelateSpeed);

                Log("Acceleate... ( Speed : " + _Speed.ToString("N1")  + " )");
            }
            else
            {
                direction = MovementSystem.PrevVelocityXZ.normalized;

                _Speed = Mathf.MoveTowards(_Speed, 0f, deltaTime * _DecelerateSpeed);

                Log("Decelerate... ( Speed : " + _Speed.ToString("N1") + " )");
            }

            _Speed = Mathf.Min(_Speed, _MaxSpeed);

            Log("Result... ( Speed : " + _Speed.ToString("N1") + " )");

            Vector3 velocity = prevVelocity + (direction * _Speed);

            velocity = Vector3.ClampMagnitude(velocity, _Speed);
            
            MovementSystem.AddVelocity(velocity);
        }
    }

}