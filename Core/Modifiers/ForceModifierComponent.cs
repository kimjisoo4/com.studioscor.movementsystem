using StudioScor.Utilities;
using UnityEngine;

namespace StudioScor.MovementSystem
{
    public interface IForceModifier : IMovementModifier
    {
        public delegate void ForceModifierEventHandler(IMovementSystem movementSystem, IForceModifier forceModifier);
        public delegate void AddForceEventHander(IMovementSystem movementSystem, IForceModifier forceModifier, Vector3 overrideAxis);

        public float Mass { get; }
        public float Drag { get; }
        public Vector3 Force { get; }
        public bool HasRemainForce { get; }
        public void SetMass(float newMass);
        public void SetDrag(float newDrag);
        public void AddForce(Vector3 addForce, Vector3 overrideAxis = default);

        public event AddForceEventHander OnAddedForce;
        public event ForceModifierEventHandler OnEndedForce;
    }

    public class ForceModifier : MovementModifier, IForceModifier
    {
        [Header(" [ Decelerate Modifier ] ")]
        [SerializeField, Min(0f)] private float _mass = 1f;
        [SerializeField] private float _drag = 1f;

        private Vector3 _force;
        private bool _hasRemainForce;
        private bool _wasPrevAddForce;


        public float Mass => _mass;
        public float Drag => _drag;
        public Vector3 Force => _force;
        public bool HasRemainForce => _hasRemainForce;

        public event IForceModifier.AddForceEventHander OnAddedForce;
        public event IForceModifier.ForceModifierEventHandler OnEndedForce;
        
        public ForceModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem, EMovementUpdateType updateType = EMovementUpdateType.Default) : base(movementSystem, moduleSystem, updateType)
        {
        }
        public void SetMass(float newMass)
        {
            _mass = Mathf.Max(0f, newMass);
        }
        public void SetDrag(float newDrag)
        {
            _drag = newDrag;
        }

        public void AddForce(Vector3 addForce, Vector3 overrideAxis = default)
        {
            _hasRemainForce = true;
            _wasPrevAddForce = true;

            Vector3 velocity = addForce /= Mathf.Max(0.001f, _mass);

            if (overrideAxis.SafeEquals(Vector3.zero))
            {
                _force += velocity;
            }
            else
            {
                _force.x = overrideAxis.x > 0 ? overrideAxis.x : _force.x + velocity.x;
                _force.y = overrideAxis.y > 0 ? overrideAxis.y : _force.y + velocity.y;
                _force.z = overrideAxis.z > 0 ? overrideAxis.z : _force.z + velocity.z;
            }


            if (velocity.y > 0f)
            {
                MovementSystem.ForceUnGrounded();
            }

            Invoke_OnAddedForce(overrideAxis);
        }

        public override void ResetModifier()
        {
            _hasRemainForce = false;
            _wasPrevAddForce = false;
            _force = default;
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

        protected override void UpdateMovement(float deltaTime)
        {
            if (!_hasRemainForce)
                return;

            MovementSystem.AddVelocity(_force);

            _force = Vector3.MoveTowards(_force, Vector3.zero, deltaTime * _drag);

            if (_wasPrevAddForce)
            {
                _wasPrevAddForce = false;
            }
            else
            {
                Vector3 prevVelocity = MovementSystem.PrevVelocity;

                _force.x = CalcForce(_force.x, prevVelocity.x);
                _force.y = CalcForce(_force.y, prevVelocity.y);
                _force.z = CalcForce(_force.z, prevVelocity.z);
            }

            if (_force.magnitude < 0.2f)
            {
                _force = default;

                _hasRemainForce = false;

                Invoke_OnEndedForce();

                return;
            }
        }

        #region Callback
        private void Invoke_OnAddedForce(Vector3 overrideAxis)
        {
            OnAddedForce?.Invoke(MovementSystem, this, overrideAxis);
        }
        private void Invoke_OnEndedForce()
        {
            OnEndedForce?.Invoke(MovementSystem, this);
        }
        #endregion
    }



    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Force Modifier", order: 30)]
    public class ForceModifierComponent : MovementModifierComponent, IForceModifier
    {
        [Header(" [ Decelerate Modifier ] ")]
        [SerializeField] private ForceModifier _modifier;
        public float Mass => _modifier.Mass;
        public float Drag => _modifier.Drag;
        public Vector3 Force => _modifier.Force;
        public bool HasRemainForce => _modifier.HasRemainForce;

        public event IForceModifier.AddForceEventHander OnAddedForce
        {
            add
            {
                _modifier.OnAddedForce += value;
            }
            remove
            {
                _modifier.OnAddedForce -= value;
            }
        }
        public event IForceModifier.ForceModifierEventHandler OnEndedForce
        {
            add
            {
                _modifier.OnEndedForce += value;
            }
            remove
            {
                _modifier.OnEndedForce -= value;
            }
        }

        public void SetMass(float newMass)
        {
            _modifier.SetMass(newMass);
        }
        public void SetDrag(float newDrag)
        {
            _modifier.SetDrag(newDrag);
        }

        public void AddForce(Vector3 addForce, Vector3 overrideAxis = default)
        {
            _modifier.AddForce(addForce, overrideAxis);
        }
        public override void ResetModifier()
        {
            _modifier.ResetModifier();
        }
        protected override void UpdateMovement(float deltaTime)
        {
            _modifier.ProcessMovement(deltaTime);
        }
    }
}