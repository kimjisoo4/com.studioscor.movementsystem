using UnityEngine;

namespace KimScor.Movement
{
    public abstract class MovementSystem : MonoBehaviour
    {
        #region EventHandler
        public delegate void ChangedMovementHandler(MovementSystem movementSystem);
        #endregion

        [Header("[Debug Mode]")]
        [SerializeField] protected bool DebugMode = false;

        protected float _MoveStrength;
        protected float _CurrentSpeed;
        protected float _CurrentVerticalSpeed;
        protected Vector3 _MoveDirection;
        protected Vector3 _LastMoveDirection;
        protected Vector3 _Velocity;
        protected Vector3 _DeltaVelocity;
        protected Vector3 _LastVelocity;
        protected bool _IsGrounded = true;
        protected bool _IsMoving = false;

        public virtual bool GetIsGrounded => _IsGrounded;
        public virtual bool GetIsMoving => _IsMoving;
        public virtual float GetMoveStrength => _MoveStrength;
        public virtual float GetCurrentSpeed => _CurrentSpeed;
        public virtual float GetCurrentVerticalSpeed => _CurrentVerticalSpeed;
        public virtual Vector3 GetMoveDirection => _MoveDirection;
        public virtual Vector3 GetLastMoveDirection => _LastMoveDirection;
        public virtual Vector3 GetVelocity => _Velocity;
        public virtual Vector3 GetDeltaVelocity => _DeltaVelocity;
        public virtual Vector3 GetLastVelocity => _LastVelocity;

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
            MovementUpdate(deltaTime);

            PropertyUpdate();
        }

        protected abstract bool CheckOnGrounded();
        protected abstract void MovementUpdate(float deltaTime);

        public abstract void SetMovePosition(Vector3 velocity);

        protected virtual void PropertyUpdate()
        {
            _LastVelocity = GetVelocity + GetDeltaVelocity;

            Vector3 currentVelocity = GetLastVelocity;

            float currentVerticalSpeed = currentVelocity.y;

            currentVelocity.y = 0f;

            float currentSpeed = currentVelocity.magnitude;


            if (GetIsMoving && currentSpeed == 0)
            {
                _IsMoving = false;

                OnFinishMovement();
            }
            else if (!GetIsMoving && currentSpeed > 0)
            {
                _IsMoving = true;

                OnStartMovement();
            }

            _CurrentSpeed = currentSpeed;
            _CurrentVerticalSpeed = currentVerticalSpeed;

            SetGrounded(CheckOnGrounded());

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

        protected void DebugText(string Text)
        {
            Debug.Log("[ " + gameObject.name + " ] " + Text);
        }

        #region EventCallBack

        public virtual void OnLand()
        {
            if (DebugMode)
                DebugText("Landed");

            OnLanded?.Invoke(this);
        }
        public virtual void OnJump()
        {
            if (DebugMode)
                DebugText("Jumped");

            OnJumped?.Invoke(this);
        }

        public virtual void OnStartMovement()
        {
            if (DebugMode)
                DebugText("Started Movement");

            OnStartedMovement?.Invoke(this);
        }
        public virtual void OnFinishMovement()
        {
            if (DebugMode)
                DebugText("Finished Movement");

            OnFinishedMovement?.Invoke(this);
        }
        #endregion
    }
}