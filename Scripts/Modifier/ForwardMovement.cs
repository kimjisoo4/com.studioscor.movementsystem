using UnityEngine;


namespace KimScor.Movement
{
    public abstract class ForwardMovement : MovementModifier
    {
        public abstract float MoveSpeed { get; }
        public abstract float Strength { get; }
        public abstract float CurrentSpeed { get; }
        public abstract float AccelateSpeed { get; }
        public abstract Transform Transform { get; }


        public override Vector3 OnMovement(float deltaTime)
        {
            float targetSpeed = MoveSpeed * Strength;

            float currentSpeed = CurrentSpeed;

            float speed = Mathf.MoveTowards(currentSpeed, targetSpeed, AccelateSpeed * deltaTime);


            Vector3 MoveVelocity = Transform.forward * speed;


            return MoveVelocity;
        }
    }
}