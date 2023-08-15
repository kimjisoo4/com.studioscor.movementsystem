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

        public event ChangedMovementHandler OnStartedInput;
        public event ChangedMovementHandler OnFinishedInput;
    }

    [DefaultExecutionOrder(MovementSystemxcutionOrder.MAIN_ORDER)]
    [AddComponentMenu("StudioScor/MovementSystem/MovementSystem", order : 0)]
    public abstract class MovementSystemComponent : BaseMonoBehaviour, IMovementSystem, IMovementEvent, IMovementModuleSystem
    {
        [Header(" [ Movement System ] ")]
        // Grounded  
        private bool isGrounded;
        private bool wasGrounded;
        private float groundDistance;
        private Vector3 groundPoint;
        private Vector3 groundNormal;
        public bool IsGrounded => isGrounded;
        public bool WasGrounded => wasGrounded;
        public float GroundDistance => groundDistance;
        public Vector3 GroundPoint => groundPoint;
        public Vector3 GroundNormal => groundNormal;

        // Modifier
        protected readonly List<IMovementModifier> modifiers = new();

        // Input
        protected Vector3 moveDirection;
        protected float moveStrength;
        public Vector3 MoveDirection => moveDirection;
        public float MoveStrength => moveStrength;

        protected Vector3 addVelocity;
        protected Vector3 addPosition;

        protected Vector3 Velocity => addVelocity;
        protected Vector3 Position => addPosition;

        // State
        protected bool isMoving;
        protected Vector3 prevVelocity;
        protected Vector3 prevVelocityXZ;
        protected float prevSpeed;
        protected float prevGravity;

        private bool shouldSortModifiers = false;
        public abstract Vector3 LastVelocity { get; }
        public bool IsMoving => isMoving;
        public Vector3 PrevVelocity => prevVelocity;
        public Vector3 PrevVelocityXZ => prevVelocityXZ;
        public float PrevSpeed => prevSpeed;
        public float PrevGravity => prevGravity;
        public IReadOnlyList<IMovementModifier> Modifiers => modifiers;

        // Events
        public event ChangedMovementHandler OnLanded;
        public event ChangedMovementHandler OnJumped;
        public event ChangedMovementHandler OnStartedMovement;
        public event ChangedMovementHandler OnFinishedMovement;
        public event ChangedMovementHandler OnStartedInput;
        public event ChangedMovementHandler OnFinishedInput;

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

            float prevStrength = moveStrength;
            
            if (direction == Vector3.zero)
            {
                moveDirection = default;
                moveStrength = 0;

                if (prevStrength > 0f)
                {
                    Callback_OnFinishedInput();
                }
            }
            else
            {
                moveDirection = direction;

                if (strength < 0)
                {
                    moveStrength = 1;
                }
                else
                {
                    moveStrength = Mathf.Clamp01(strength);
                }

                if (prevStrength <= 0f)
                {
                    Callback_OnStartedInput();
                }
            }
        }

        public void AddModifier(IMovementModifier modifier)
        {
            if (modifier is null || modifiers.Contains(modifier))
                return;

            modifiers.Add(modifier);

            if(modifiers.Count >= 2)
                shouldSortModifiers = true;

            
        }
        public void RemoveModifier(IMovementModifier modifier)
        {
            if (modifier is null)
                return;

            modifiers.Remove(modifier);
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
            addVelocity += velocity;
        }
        public void MovePosition(Vector3 addPosition)
        {
            this.addPosition += addPosition;
        }

        public void ForceOnGrounded()
        {
            bool prevGrounded = isGrounded;

            wasGrounded = true;
            isGrounded = true;

            if (!prevGrounded)
            {
                OnLand();
                Callback_OnLanded();
            }
        }
        public void ForceUnGrounded()
        {
            bool prevGrounded = isGrounded;

            wasGrounded = false;
            isGrounded = false;

            SetGroundState(Vector3.zero, Vector3.up, 0f);

            if (prevGrounded)
            {
                OnJump();
                Callback_OnJumped();
            }
        }
        public void SetGrounded(bool isGrounded)
        {
            wasGrounded = this.isGrounded;
            this.isGrounded = isGrounded;

            if (wasGrounded == this.isGrounded)
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
            groundPoint = point;
            groundNormal = normal;
            groundDistance = distance;
        }

        public void UpdateMovement(float deltaTime)
        {
            if(shouldSortModifiers)
            {
                shouldSortModifiers = false;

                modifiers.Sort(SortModifier);
            }

            foreach (var modifier in modifiers)
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
            addVelocity = default;
            addPosition = default;
        }

        protected void CheckMoving()
        {
            if (isMoving && PrevSpeed == 0)
            {
                isMoving = false;

                OnFinishMovement();
                Callback_OnFinishdMovement();
            }
            else if (!isMoving && PrevSpeed > 0)
            {
                isMoving = true;

                OnStartMovement();
                Callback_OnStartedMovement();
            }
        }

        public abstract void Teleport(Vector3 position);
        protected abstract void OnMovement(float deltaTime);
        protected virtual void PropertyUpdate()
        {
            Vector3 velocity = LastVelocity;

            prevVelocity = velocity;
            prevGravity = IsGrounded? 0f : velocity.y;

            velocity.y = 0;

            prevVelocityXZ = velocity;
            prevSpeed = prevVelocityXZ.magnitude;
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