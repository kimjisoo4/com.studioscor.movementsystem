using UnityEngine;
using System.Collections.Generic;
using System;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public delegate void ChangedMovementHandler(IMovementEvent movementSystem);

    public static class MovementSystemUtility
    {
        #region Get MovementSystem
        public static IMovementSystem GetMovementSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<IMovementSystem>();
        }
        public static IMovementSystem GetMovementSystem(this Component component)
        {
            var movementSystem = component as IMovementSystem;

            if (movementSystem is not null)
                return movementSystem;

            return component.GetComponent<IMovementSystem>();
        }
        public static bool TryGetMovementSystem(this GameObject gameObject, out IMovementSystem movementSystem)
        {
            return gameObject.TryGetComponent(out movementSystem);
        }
        public static bool TryGetMovementSystem(this Component component, out IMovementSystem movementSystem)
        {
            movementSystem = component as IMovementSystem;

            if (movementSystem is not null)
                return true;

            return component.TryGetComponent(out movementSystem);
        }
        #endregion

        #region Get MovementEvent
        public static IMovementEvent GetMovementEvent(this GameObject gameObject)
        {
            return gameObject.GetComponent<IMovementEvent>();
        }
        public static IMovementEvent GetMovementEvent(this Component component)
        {
            var movementSystem = component as IMovementEvent;

            if (movementSystem is not null)
                return movementSystem;

            return component.GetComponent<IMovementEvent>();
        }
        public static bool TryGetMovementEvent(this GameObject gameObject, out IMovementEvent movementEvent)
        {
            return gameObject.TryGetComponent(out movementEvent);
        }
        public static bool TryGetMovementEvent(this Component component, out IMovementEvent movementEvent)
        {
            movementEvent = component as IMovementEvent;

            if (movementEvent is not null)
                return true;

            return component.TryGetComponent(out movementEvent);
        }
        #endregion
        #region Get Movement Module System
        public static IMovementModuleSystem GetMovementModuleSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<IMovementModuleSystem>();
        }
        public static IMovementModuleSystem GetMovementModuleSystem(this Component component)
        {
            var movementSystem = component as IMovementModuleSystem;

            if (movementSystem is not null)
                return movementSystem;

            return component.GetComponent<IMovementModuleSystem>();
        }
        public static bool TryGetMovementModuleSystem(this GameObject gameObject, out IMovementModuleSystem movementModuleSystem)
        {
            return gameObject.TryGetComponent(out movementModuleSystem);
        }
        public static bool TryGetMovementModuleSystem(this Component component, out IMovementModuleSystem movementModuleSystem)
        {
            movementModuleSystem = component as IMovementModuleSystem;

            if (movementModuleSystem is not null)
                return true;

            return component.TryGetComponent(out movementModuleSystem);
        }
        #endregion

        public static bool TryGetModifier<T>(this IMovementModuleSystem movementModuleSystem, out T movementModifier) where T : IMovementModifier
        {
            foreach (var modifier in movementModuleSystem.Modifiers)
            {
                if (modifier.GetType() == typeof(T))
                {
                    movementModifier = (T)modifier;

                    return true;
                }
            }
            
            movementModifier = default(T);

            return false;
        }
    }

    public interface IMovementModuleSystem
    {
        public IReadOnlyList<IMovementModifier> Modifiers { get; }

        public void AddModifier(IMovementModifier movementModifier);
        public void RemoveModifier(IMovementModifier movementModifier);
    }

    public interface IMovementSystem
    {
        public Transform transform { get; }
        public GameObject gameObject { get; }

        public float MoveStrength { get; }
        public Vector3 MoveDirection { get; }
        public bool IsGrounded { get; }
        public bool IsMoving { get; }

        public float GroundDistance { get; }

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
        public void Teleport(Vector3 position = default);
        public void UpdateMovement(float deltaTime);
    }

    public interface IMovementEvent
    {
        public event ChangedMovementHandler OnLanded;
        public event ChangedMovementHandler OnJumped;
        public event ChangedMovementHandler OnStartedMovement;
        public event ChangedMovementHandler OnFinishedMovement;
    }

    [DefaultExecutionOrder(MovementSystemxcutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/MovementSystem/MovementSystem", order : 0)]
    public abstract class MovementSystemComponent : BaseMonoBehaviour, IMovementSystem, IMovementEvent, IMovementModuleSystem
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
        protected readonly List<IMovementModifier> _Modifiers = new();

        // Input
        protected Vector3 _MoveDirection;
        protected float _MoveStrength;
        public Vector3 MoveDirection => _MoveDirection;
        public float MoveStrength => _MoveStrength;

        protected Vector3 _AddVelocity;
        protected Vector3 _AddPosition;

        protected Vector3 Velocity => _AddVelocity;
        protected Vector3 Position => _AddPosition;

        // State
        protected bool _IsMoving;
        protected Vector3 _PrevVelocity;
        protected Vector3 _PrevVelocityXZ;
        protected float _PrevSpeed;
        protected float _PrevGravity;

        private bool shouldSortModifiers = false;
        public abstract Vector3 LastVelocity { get; }
        public bool IsMoving => _IsMoving;
        public Vector3 PrevVelocity => _PrevVelocity;
        public Vector3 PrevVelocityXZ => _PrevVelocityXZ;
        public float PrevSpeed => _PrevSpeed;
        public float PrevGravity => _PrevGravity;
        public IReadOnlyList<IMovementModifier> Modifiers => _Modifiers;

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
            if (modifier is null || _Modifiers.Contains(modifier))
                return;

            _Modifiers.Add(modifier);

            if(_Modifiers.Count >= 2)
                shouldSortModifiers = true;

            
        }
        public void RemoveModifier(IMovementModifier modifier)
        {
            if (modifier is null)
                return;

            _Modifiers.Remove(modifier);
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
            {
                OnLand();
                Callback_OnLanded();
            }
        }
        public void ForceUnGrounded()
        {
            bool prevGrounded = _IsGrounded;

            _WasGrounded = false;
            _IsGrounded = false;

            SetGroundState(Vector3.zero, Vector3.up, 0f);

            if (prevGrounded)
            {
                OnJump();
                Callback_OnJumped();
            }
        }
        public void SetGrounded(bool isGrounded)
        {
            _WasGrounded = _IsGrounded;
            _IsGrounded = isGrounded;

            if (_WasGrounded == _IsGrounded)
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
            _GroundPoint = point;
            _GroundNormal = normal;
            _GroundDistance = distance;
        }

        public void UpdateMovement(float deltaTime)
        {
            if(shouldSortModifiers)
            {
                shouldSortModifiers = false;

                _Modifiers.Sort(SortModifier);
            }

            foreach (var modifier in _Modifiers)
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

                OnFinishMovement();
                Callback_OnFinishdMovement();
            }
            else if (!_IsMoving && PrevSpeed > 0)
            {
                _IsMoving = true;

                OnStartMovement();
                Callback_OnStartedMovement();
            }
        }

        public abstract void Teleport(Vector3 position);
        protected abstract void OnMovement(float deltaTime);
        protected virtual void PropertyUpdate()
        {
            Vector3 velocity = LastVelocity;

            _PrevVelocity = velocity;
            _PrevGravity = IsGrounded? 0f : velocity.y;

            velocity.y = 0;

            _PrevVelocityXZ = velocity;
            _PrevSpeed = _PrevVelocityXZ.magnitude;
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
        #endregion
    }

}