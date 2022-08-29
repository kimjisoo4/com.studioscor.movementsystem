using UnityEngine;
using System.Diagnostics;

namespace KimScor.MovementSystem
{
    public abstract class MovementSystem : MonoBehaviour
    {
        #region EventHandler
        public delegate void ChangedMovementHandler(MovementSystem movementSystem);
        #endregion

        [Header("[Debug Mode]")]
        [SerializeField] protected bool DebugMode = false;

        protected float _MoveStrength = 0f;
        protected float _CurrentSpeed = 0f;
        protected float _CurrentVerticalSpeed = 0f;
        protected Vector3 _MoveDirection = Vector3.zero;
        protected Vector3 _LastMoveDirection = Vector3.zero;
        protected Vector3 _Velocity = Vector3.zero;
        protected Vector3 _DeltaVelocity = Vector3.zero;
        protected Vector3 _LastVelocity = Vector3.zero;
        protected bool _IsGrounded = true;
        protected bool _IsMoving = false;
        
        public virtual bool IsGrounded => _IsGrounded;
        public virtual bool IsMoving => _IsMoving;
        public virtual float MoveStrength => _MoveStrength;
        public virtual float CurrentSpeed => _CurrentSpeed;
        public virtual float CurrentVerticalSpeed => _CurrentVerticalSpeed;
        public virtual Vector3 MoveDirection => _MoveDirection;
        public virtual Vector3 LastMoveDirection => _LastMoveDirection;
        public virtual Vector3 Velocity => _Velocity;
        public virtual Vector3 DeltaVelocity => _DeltaVelocity;
        public virtual Vector3 LastVelocity => _LastVelocity;

        #region events
        public event ChangedMovementHandler OnLanded;
        public event ChangedMovementHandler OnJumped;
        public event ChangedMovementHandler OnStartedMovement;
        public event ChangedMovementHandler OnFinishedMovement;
        #endregion


        public void SetMoveDirection(Vector3 direction, float strength = -1f)
        {
            if (direction == Vector3.zero)
            {
                _MoveDirection = default;
                _MoveStrength = 0;
            }
            else
            {
                _MoveDirection = direction;
                _LastMoveDirection = _MoveDirection;

                if (strength < 0)
                {
                    _MoveStrength = 1;
                }
                else
                {
                    _MoveStrength = Mathf.Clamp01(strength);
                }
            }
        }

        public void OnMovement(float deltaTime)
        {
            SetGrounded(CheckOnGrounded());

            MovementUpdate(deltaTime);

            PropertyUpdate();
        }

        protected abstract bool CheckOnGrounded();
        protected abstract void MovementUpdate(float deltaTime);
        public abstract void SetMovePosition(Vector3 position);

        protected virtual void PropertyUpdate()
        {
            _LastVelocity = Velocity + DeltaVelocity;

            Vector3 currentVelocity = LastVelocity;

            float currentVerticalSpeed = currentVelocity.y;

            currentVelocity.y = 0f;

            float currentSpeed = currentVelocity.magnitude;


            if (IsMoving && currentSpeed == 0)
            {
                _IsMoving = false;

                OnFinishMovement();
            }
            else if (!IsMoving && currentSpeed > 0)
            {
                _IsMoving = true;

                OnStartMovement();
            }

            _CurrentSpeed = currentSpeed;
            _CurrentVerticalSpeed = currentVerticalSpeed;

            ResetVelocity();
        }

        public void AddVelocity(Vector3 velocity)
        {
            _Velocity += velocity;
        }

        public void AddDeltaVelocity(Vector3 velocity)
        {
            _DeltaVelocity += velocity;
        }

        public void ResetVelocity()
        {
            _Velocity = Vector3.zero;
            _DeltaVelocity = Vector3.zero;
        }

        public void SetGrounded(bool isGrounded)
        {
            if (_IsGrounded != isGrounded)
            {
                _IsGrounded = isGrounded;

                if (isGrounded)
                {
                    OnLand();
                }
                else
                {
                    OnJump();
                }
            }
        }

        #region EDITOR
        [Conditional("UNITY_EDITOR")]
        protected void Log(string log)
        {
            if (DebugMode)
                UnityEngine.Debug.Log("MovementSystem [" + gameObject.name + "] : " + log);
        }
        #endregion

        #region EventCallBack

        protected virtual void OnLand()
        {
            if (DebugMode)
                Log("Landed");

            OnLanded?.Invoke(this);
        }
        protected virtual void OnJump()
        {
            if (DebugMode)
                Log("Jumped");

            OnJumped?.Invoke(this);
        }

        protected virtual void OnStartMovement()
        {
            if (DebugMode)
                Log("Started Movement");

            OnStartedMovement?.Invoke(this);
        }
        protected virtual void OnFinishMovement()
        {
            if (DebugMode)
                Log("Finished Movement");

            OnFinishedMovement?.Invoke(this);
        }
        #endregion
    }
}