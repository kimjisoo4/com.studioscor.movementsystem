using UnityEngine;
using System.Collections.Generic;
using System;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public delegate void ChangedMovementHandler(IMovementSystemEvent movementSystem);

    public interface IMovementSystem
    {
        public Transform transform { get; }
        public GameObject gameObject { get; }

        public float MoveStrength { get; }
        public Vector3 MoveDirection { get; }
        public bool IsGrounded { get; }

        public void SetGrounded(bool isGrounded);
        public void Teleport(Vector3 position = default);

        public void AddVelocity(Vector3 velocity);
        public void MovePosition(Vector3 position);
    }

    public enum EMovementSystemEventType
    {
        OnJumped,
        OnLanded,
        OnStartedMovement,
        OnFinishedMovement,
    }

    public interface IMovementSystemEvent
    {
        public event ChangedMovementHandler OnLanded;
        public event ChangedMovementHandler OnJumped;
        public event ChangedMovementHandler OnStartedMovement;
        public event ChangedMovementHandler OnFinishedMovement;
    }

    [DefaultExecutionOrder(MovementSystemxcutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/MovementSystem/MovementSystem", order : 0)]
    public abstract class MovementSystemComponent : BaseMonoBehaviour, IMovementSystem, IMovementSystemEvent
    {
       

        [Header(" [ Movement System ] ")]
        // Grounded  
        private bool _IsGrounded;
        private bool _WasGrounded;
        private float _GroundDistance;
        private Vector3 _GroundPoint;
        private Vector3 _GroundNormal;
        public bool IsGrounded => _IsGrounded;
        public bool WasGrounded => _WasGrounded;
        public float GroundDistance => _GroundDistance;
        public Vector3 GroundPoint => _GroundPoint;
        public Vector3 GroundNormal => _GroundNormal;

        // Modifier
        protected List<IMovementModifier> _EarlyModifiers;
        protected List<IMovementModifier> _DefaultModifiers;
        protected List<IMovementModifier> _LateModifiers;

        // Input
        protected Vector3 _MoveDirection;
        protected float _MoveStrength;
        public Vector3 MoveDirection => _MoveDirection;
        public float MoveStrength => _MoveStrength;

        protected Vector3 _AddVelocity;
        protected Vector3 _AddPosition;

        public Vector3 MomentVelocity => _AddVelocity;
        public Vector3 MomentPosition => _AddPosition;

        // State
        protected bool _IsMoving;
        protected Vector3 _PrevVelocity;
        protected Vector3 _PrevVelocityXZ;
        protected float _PrevSpeed;
        protected float _PrevGravity;

        protected abstract Vector3 LastVelocity { get; }
        public bool IsMoving => _IsMoving;
        public Vector3 PrevVelocity => _PrevVelocity;
        public Vector3 PrevVelocityXZ => _PrevVelocityXZ;
        public float PrevSpeed => _PrevSpeed;
        public float PrevGravity => _PrevGravity;

        // Events
        public event ChangedMovementHandler OnLanded;
        public event ChangedMovementHandler OnJumped;
        public event ChangedMovementHandler OnStartedMovement;
        public event ChangedMovementHandler OnFinishedMovement;

        private void Awake()
        {
            Setuo();
        }

        private void Setuo()
        {
            _EarlyModifiers = new();
            _DefaultModifiers = new();
            _LateModifiers = new();

            OnSetup();
        }

        protected virtual void OnSetup() { }

        public void SetMoveDirection(Vector3 direction, float strength = -1f)
        {
            Log("Move Direction - " + direction + " Strength - " + strength.ToString("N1"));

            if (direction == Vector3.zero)
            {
                _MoveDirection = default;
                _MoveStrength = 0;
            }
            else
            {
                _MoveDirection = direction;

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

        public void AddModifier(IMovementModifier modifier)
        {
            switch (modifier.UpdateType)
            {
                case EMovementUpdateType.Early:
                    _EarlyModifiers.Add(modifier);
                    break;
                case EMovementUpdateType.Default:
                    _DefaultModifiers.Add(modifier);
                    break;
                case EMovementUpdateType.Late:
                    _LateModifiers.Add(modifier);
                    break;
                default:
                    break;
            }
        }
        public void RemoveModifier(IMovementModifier modifier)
        {
            switch (modifier.UpdateType)
            {
                case EMovementUpdateType.Early:
                    _EarlyModifiers.Remove(modifier);
                    break;
                case EMovementUpdateType.Default:
                    _DefaultModifiers.Remove(modifier);
                    break;
                case EMovementUpdateType.Late:
                    _LateModifiers.Remove(modifier);
                    break;
                default:
                    break;
            }
        }

        public bool FindModifier(Type modifierType, out IMovementModifier movementModifier)
        {
            foreach (var modifier in _DefaultModifiers)
            {
                if (modifier.GetType() == modifierType)
                {
                    movementModifier = modifier;

                    return true;
                }
            }
            foreach (var modifier in _EarlyModifiers)
            {
                if (modifier.GetType() == modifierType)
                {
                    movementModifier = modifier;

                    return true;
                }
            }
            foreach (var modifier in _LateModifiers)
            {
                if (modifier.GetType() == modifierType)
                {
                    movementModifier = modifier;

                    return true;
                }
            }

            movementModifier = null;

            return false;
        }

        
        public void AddVelocity(Vector3 velocity)
        {
            _AddVelocity += velocity;
        }
        public void MovePosition(Vector3 addPosition)
        {
            _AddPosition += addPosition;
        }

        public void ForceOnGrounded()
        {
            bool prevGrounded = _IsGrounded;

            _WasGrounded = true;
            _IsGrounded = true;

            if (!prevGrounded)
                Callback_OnLanded();
        }
        public void ForceUnGrounded()
        {
            bool prevGrounded = _IsGrounded;

            _WasGrounded = false;
            _IsGrounded = false;

            SetGroundState(Vector3.zero, Vector3.up, 0f);

            if (prevGrounded)
                Callback_OnJumped();
        }
        public void SetGrounded(bool isGrounded)
        {
            _WasGrounded = _IsGrounded;
            _IsGrounded = isGrounded;

            if (_WasGrounded == _IsGrounded)
                return;

            if (IsGrounded)
            {
                Callback_OnLanded();
            }
            else
            {
                Callback_OnJumped();
            }
        }
        public void SetGroundState(Vector3 point, Vector3 normal, float distance)
        {
            _GroundPoint = point;
            _GroundNormal = normal;
            _GroundDistance = distance;
        }

        public void UpdateMovement(float deltaTime)
        {
            foreach (var modifier in _EarlyModifiers)
            {
                modifier.ProcessMovement(deltaTime);
            }
            foreach (var modifier in _DefaultModifiers)
            {
                modifier.ProcessMovement(deltaTime);
            }
            foreach (var modifier in _LateModifiers)
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
            _AddVelocity = default;
            _AddPosition = default;
        }

        protected void CheckMoving()
        {
            if (_IsMoving && PrevSpeed == 0)
            {
                _IsMoving = false;

                Callback_OnFinishdMovement();
            }
            else if (!_IsMoving && PrevSpeed > 0)
            {
                _IsMoving = true;

                Callback_OnStartedMovement();
            }
        }

        public abstract void Teleport(Vector3 position);
        protected abstract void OnMovement(float deltaTime);
        protected virtual void PropertyUpdate()
        {
            Vector3 velocity = LastVelocity;

            _PrevVelocity = velocity;
            _PrevGravity = velocity.y;

            velocity.y = 0;

            _PrevVelocityXZ = velocity;
            _PrevSpeed = _PrevVelocityXZ.magnitude;
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
        #endregion
    }

}