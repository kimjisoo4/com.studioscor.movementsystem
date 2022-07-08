using UnityEngine;

namespace KimScor.Movement
{
    public abstract class AddForceMovement : MovementModifier
    {
        public abstract float Mass { get; }
        public abstract float Drag { get; }

        private Vector3 _Force;

        private bool _RemainForce = false;

        public void OverrideForce(Vector3 newForce)
        {
            _Force = newForce / Mass;

            _RemainForce = true;
        }
        public void AddForce(Vector3 addforce)
        {
            _Force += addforce / Mass;

            _RemainForce = true;
        }
        public void ResetForce()
        {
            _Force = Vector3.zero;

            _RemainForce = false;
        }

        public override Vector3 OnMovement(float deltaTime)
        {
            if (!_RemainForce)
            {
                return Vector3.zero;
            }

            float power = _Force.magnitude;

            if (power < 0.2f)
            {
                ResetForce();

                return Vector3.zero;
            }

            Vector3 velocity = _Force;

            _Force = Vector3.MoveTowards(_Force, Vector3.zero, deltaTime * Drag);

            return velocity;
        }
    }
    public abstract class DirectionMovement : MovementModifier
    {
        public abstract float MoveSpeed { get; }
        public abstract float Strength { get; }

        public abstract float AccelateSpeed { get; }
        public abstract float DecelerateSpeed { get; }

        public abstract Vector3 MoveDirection { get; }
        public abstract Vector3 LastMoveDirection { get; }


        private float _CurrentSpeed = 0f;

        public void SetCurrentSpeed(float newCurrentSpeed)
        {
            _CurrentSpeed = newCurrentSpeed;
        }
        public void ResetCurrentSpeed()
        {
            _CurrentSpeed = 0f;
        }

        public override Vector3 OnMovement(float deltaTime)
        {
            float targetSpeed = MoveSpeed * Strength;

            float currentSpeed = _CurrentSpeed;

            Vector3 direction = MoveDirection;

            float accelateSpeed;


            if (direction == Vector3.zero)
            {
                direction = LastMoveDirection;

                accelateSpeed = DecelerateSpeed;
            }
            else
            {
                if (currentSpeed > MoveSpeed)
                {
                    accelateSpeed = DecelerateSpeed;
                }
                else
                {
                    accelateSpeed = AccelateSpeed;
                }
            }


            _CurrentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelateSpeed * deltaTime);


            Vector3 MoveVelocity = direction * _CurrentSpeed;


            return MoveVelocity;
        }

    }
}