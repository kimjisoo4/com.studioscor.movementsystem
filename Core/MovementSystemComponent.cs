using UnityEngine;
using System.Collections.Generic;
using System;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public interface IMovementModuleSystem
    {
        public IReadOnlyList<IMovementModifier> Modifiers { get; }

        public void AddModifier(IMovementModifier movementModifier);
        public void RemoveModifier(IMovementModifier movementModifier);
    }

    public interface IMovementSystem
    {
        public delegate void ChangedMovementHandler(IMovementSystem movementSystem);
        public Transform transform { get; }
        public GameObject gameObject { get; }

        public float MoveStrength { get; }
        public Vector3 MoveDirection { get; }
        public bool IsGrounded { get; }
        public bool IsMoving { get; }

        public float GroundDistance { get; }
        public Vector3 GroundNormal { get; }
        public Vector3 GroundPoint { get;  }

        public Vector3 PrevVelocity { get; }
        public Vector3 PrevVelocityXZ { get; }
        public float PrevSpeed { get; }
        public float PrevGravity { get; }

        public void ForceOnGrounded();
        public void ForceUnGrounded();
        public void SetGrounded(bool isGrounded);
        public void SetGroundState(Vector3 point, Vector3 normal, float distance);

        public void SetMoveDirection(Vector3 direction, float stregnth = -1f);
        public void AddVelocity(Vector3 velocity);
        public void MovePosition(Vector3 position);
        public void Teleport(Vector3 position = default, bool isImmediately = true);
        public void UpdateMovement(float deltaTime);

        public event ChangedMovementHandler OnLanded;
        public event ChangedMovementHandler OnJumped;

        public event ChangedMovementHandler OnStartedMovement;
        public event ChangedMovementHandler OnFinishedMovement;

        public event ChangedMovementHandler OnStartedInput;
        public event ChangedMovementHandler OnFinishedInput;
    }


    [DefaultExecutionOrder(MovementSystemxcutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/MovementSystem/MovementSystem", order : 0)]
    public abstract class MovementSystemComponent : BaseMonoBehaviour, IMovementSystem, IMovementModuleSystem
    {
        [Header(" [ Movement System ] ")]
        // Grounded  
        private bool _isGrounded;
        private bool _wasGrounded;
        private float _groundDistance;
        private Vector3 _groundPoint;
        private Vector3 _groundNormal;
        public bool IsGrounded => _isGrounded;
        public bool WasGrounded => _wasGrounded;
        public float GroundDistance => _groundDistance;
        public Vector3 GroundPoint => _groundPoint;
        public Vector3 GroundNormal => _groundNormal;

        // Modifier
        protected readonly List<IMovementModifier> _modifiers = new();

        // Input
        protected Vector3 _moveDirection;
        protected float _moveStrength;
        public Vector3 MoveDirection => _moveDirection;
        public float MoveStrength => _moveStrength;

        protected Vector3 _addVelocity;
        protected Vector3 _addPosition;

        protected Vector3 Velocity => _addVelocity;
        protected Vector3 Position => _addPosition;

        // State
        protected bool _isMoving;
        protected Vector3 _prevVelocity;
        protected Vector3 _prevVelocityXZ;
        protected float _prevSpeed;
        protected float _prevGravity;

        private bool _shouldSortModifiers = false;
        public abstract Vector3 LastVelocity { get; }
        public bool IsMoving => _isMoving;
        public Vector3 PrevVelocity => _prevVelocity;
        public Vector3 PrevVelocityXZ => _prevVelocityXZ;
        public float PrevSpeed => _prevSpeed;
        public float PrevGravity => _prevGravity;
        public IReadOnlyList<IMovementModifier> Modifiers => _modifiers;

        // Events
        public event IMovementSystem.ChangedMovementHandler OnLanded;
        public event IMovementSystem.ChangedMovementHandler OnJumped;
        public event IMovementSystem.ChangedMovementHandler OnStartedMovement;
        public event IMovementSystem.ChangedMovementHandler OnFinishedMovement;
        public event IMovementSystem.ChangedMovementHandler OnStartedInput;
        public event IMovementSystem.ChangedMovementHandler OnFinishedInput;

        private void Awake()
        {
            Setuo();
        }

        private void Setuo()
        {
            OnSetup();
        }

        protected virtual void OnSetup() { }

        public void SetMoveDirection(Vector3 direction, float strength = -1f)
        {
            Log("Move Direction - " + direction + " Strength - " + strength.ToString("N1"));

            float prevStrength = _moveStrength;
            
            if (direction == Vector3.zero)
            {
                _moveDirection = default;
                _moveStrength = 0;

                if (prevStrength > 0f)
                {
                    Callback_OnFinishedInput();
                }
            }
            else
            {
                _moveDirection = direction;

                if (strength < 0)
                {
                    _moveStrength = 1;
                }
                else
                {
                    _moveStrength = Mathf.Clamp01(strength);
                }

                if (prevStrength <= 0f)
                {
                    Callback_OnStartedInput();
                }
            }
        }

        public void AddModifier(IMovementModifier modifier)
        {
            if (modifier is null || _modifiers.Contains(modifier))
                return;

            _modifiers.Add(modifier);

            if(_modifiers.Count >= 2)
                _shouldSortModifiers = true;

            
        }
        public void RemoveModifier(IMovementModifier modifier)
        {
            if (modifier is null)
                return;

            _modifiers.Remove(modifier);
        }

        private int SortModifier(IMovementModifier lhs, IMovementModifier rhs)
        {
            if(lhs.UpdateType < rhs.UpdateType)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
        
        public void AddVelocity(Vector3 velocity)
        {
            _addVelocity += velocity;
        }
        public void MovePosition(Vector3 addPosition)
        {
            this._addPosition += addPosition;
        }

        public void ForceOnGrounded()
        {
            bool prevGrounded = _isGrounded;

            _wasGrounded = true;
            _isGrounded = true;

            if (!prevGrounded)
            {
                OnLand();
                Callback_OnLanded();
            }
        }
        public void ForceUnGrounded()
        {
            bool prevGrounded = _isGrounded;

            _wasGrounded = false;
            _isGrounded = false;

            SetGroundState(Vector3.zero, Vector3.up, 0f);

            if (prevGrounded)
            {
                OnJump();
                Callback_OnJumped();
            }
        }
        public void SetGrounded(bool isGrounded)
        {
            _wasGrounded = this._isGrounded;
            this._isGrounded = isGrounded;

            if (_wasGrounded == this._isGrounded)
                return;

            if (IsGrounded)
            {
                OnLand();
                Callback_OnLanded();
            }
            else
            {
                OnJump();
                Callback_OnJumped();
            }
        }
        public void SetGroundState(Vector3 point, Vector3 normal, float distance)
        {
            _groundPoint = point;
            _groundNormal = normal;
            _groundDistance = distance;
        }

        public void UpdateMovement(float deltaTime)
        {
            if(_shouldSortModifiers)
            {
                _shouldSortModifiers = false;

                _modifiers.Sort(SortModifier);
            }

            foreach (var modifier in _modifiers)
            {
                modifier.ProcessMovement(deltaTime);
            }

            OnMovement(deltaTime);

            PropertyUpdate();
            CheckMoving();
            ResetVelocity();
        }

        protected void ResetVelocity()
        {
            _addVelocity = default;
            _addPosition = default;
        }

        protected void CheckMoving()
        {
            if (_isMoving && PrevSpeed <= 0f)
            {
                _isMoving = false;

                OnFinishMovement();
                Callback_OnFinishdMovement();
            }
            else if (!_isMoving && PrevSpeed > 0f)
            {
                _isMoving = true;

                OnStartMovement();
                Callback_OnStartedMovement();
            }
        }

        public abstract void Teleport(Vector3 position = default, bool isImmediately = true);
        protected abstract void OnMovement(float deltaTime);
        protected virtual void PropertyUpdate()
        {
            Vector3 velocity = LastVelocity;

            _prevVelocity = velocity;
            _prevGravity = IsGrounded? 0f : velocity.y;

            velocity.y = 0;

            _prevVelocityXZ = velocity;
            _prevSpeed = _prevVelocityXZ.magnitude;
        }

        protected virtual void OnLand()
        {

        }
        protected virtual void OnJump()
        {

        }
        protected virtual void OnStartMovement()
        {

        }
        protected virtual void OnFinishMovement()
        {

        }

        #region CallBack
        protected void Callback_OnLanded()
        {
            Log("On Landed");

            OnLanded?.Invoke(this);
        }
        protected void Callback_OnJumped()
        {
            Log("On Jumped");

            OnJumped?.Invoke(this);
        }
        protected void Callback_OnStartedMovement()
        {
            Log("Started Movement");

            OnStartedMovement?.Invoke(this);
        }
        protected void Callback_OnFinishdMovement()
        {
            Log("Finished Movement");

            OnFinishedMovement?.Invoke(this);
        }
        protected void Callback_OnStartedInput()
        {
            Log("Started Input");

            OnStartedInput?.Invoke(this);
        }
        protected void Callback_OnFinishedInput()
        {
            Log("Finished Input");

            OnFinishedInput?.Invoke(this);
        }
        #endregion
    }

}