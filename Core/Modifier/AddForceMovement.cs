using UnityEngine;

namespace KimScor.MovementSystem
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
}