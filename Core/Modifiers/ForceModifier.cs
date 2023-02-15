using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Force Modifier", order: 30)]
    public class ForceModifier : Modifier
    {
        #region Event
        public delegate void ForceModifierEventHandler(MovementSystemComponent movementSystem, ForceModifier forceModifier);
        #endregion

        [Header(" [ Decelerate Modifier ] ")]
        [SerializeField, Min(0f)] private float _Mass = 1f;
        [SerializeField] private float _Drag = 1f;

        private Vector3 _Force;
        private bool _IsRemainForce;
        private bool _IsPrevAddForce;

        public Vector3 Force => _Force;
        public bool IsRemainForce => _IsRemainForce;

        public event ForceModifierEventHandler OnAddedForce;
        public event ForceModifierEventHandler OnOverridedForce;
        public event ForceModifierEventHandler OnEndedForce;

        public void SetMass(float newMass)
        {
            _Mass = Mathf.Max(0f, newMass);
        }
        public void SetDrag(float newDrag)
        {
            _Drag = newDrag;
        }

        public void AddForce(Vector3 velocity)
        {
            _IsRemainForce = true;
            _IsPrevAddForce = true;

            velocity /= Mathf.Max(0.001f, _Mass);
            _Force += velocity;

            if (velocity.y > 0f)
            {
                MovementSystem.ForceUnGrounded();
            }

            Callback_OnAddedForce();
        }
        public void OverrideForce(Vector3 velocity)
        {
            _IsRemainForce = true;
            _IsPrevAddForce = true;

            velocity /= Mathf.Max(0.001f, _Mass);
            _Force = velocity;

            if (velocity.y > 0f)
            {
                MovementSystem.ForceUnGrounded();
            }

            Callback_OnOverridedForce();
        }

        public override void ResetModifier()
        {
            _IsRemainForce = false;
            _IsPrevAddForce = false;
            _Force = default;
        }

        private float CalcForce(float force, float prev)
        {
            if (force != 0)
            {
                if (force.IsPositive() == prev.IsPositive())
                {
                    if (force.IsPositive())
                    {
                        return force > prev ? prev : force;
                    }
                    else
                    {
                        return force < prev ? prev : force;
                    }
                }
                else
                {
                    return 0f;
                }
            }

            return force;
        }

        public override void ProcessMovement(float deltaTime)
        {
            if (!_IsRemainForce)
                return;

            MovementSystem.AddVelocity(_Force);

            _Force = Vector3.MoveTowards(_Force, Vector3.zero, deltaTime * _Drag);

            if (_IsPrevAddForce)
            {
                _IsPrevAddForce = false;
            }
            else
            {
                Vector3 prevVelocity = MovementSystem.PrevVelocity;

                _Force.x = CalcForce(_Force.x, prevVelocity.x);
                _Force.y = CalcForce(_Force.y, prevVelocity.y);
                _Force.z = CalcForce(_Force.z, prevVelocity.z);
            }

            if (_Force.magnitude < 0.2f)
            {
                _Force = default;

                _IsRemainForce = false;

                Callback_OnEndedForce();

                return;
            }
        }

        #region Callback
        private void Callback_OnAddedForce()
        {
            OnAddedForce?.Invoke(MovementSystem, this);
        }
        private void Callback_OnOverridedForce()
        {
            OnOverridedForce?.Invoke(MovementSystem, this);
        }
        private void Callback_OnEndedForce()
        {
            OnEndedForce?.Invoke(MovementSystem, this);
        }
        #endregion
    }

}