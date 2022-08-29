using UnityEngine;

namespace KimScor.MovementSystem
{
    public abstract class BaseMovementSystem : MovementSystem
    {
        [Header("[Property]")]
        [SerializeField] protected float _MoveSpeed = 5f;
        [SerializeField] protected float _AcceateSpeed = 10f;
        [SerializeField] protected float _DecelerateSpeed = 10f;
        [SerializeField] protected float _Gravity = 9.81f;

        public virtual float GetMoveSpeed => _MoveSpeed;
        public virtual float GetAccelateSpeed => _AcceateSpeed;
        public virtual float GetDecelerateSpeed => _DecelerateSpeed;
        public virtual float GetGravity => _Gravity;

        [Header("[Activate Movement]")]
        [SerializeField] protected bool _UseMovement = true;
        [SerializeField] protected bool _UseSideStep = false;
        [SerializeField] protected bool _UseGravity = true;

        public bool UseMovement => _UseMovement;
        public bool UseSideStep => _UseSideStep;
        public bool UseGravity => _UseGravity;

        protected DirectionalModifier DirectionMovement;
        protected ForwardModifier ForwardMovement;
        protected GravityModifier GravityMovement;
        protected TargetMovement TargetMovement;
        public bool GetUsingTargetMovement => TargetMovement.IsActivate;
        
        protected virtual void Awake()
        {
            DirectionMovement = new DirectionalModifier(this);
            ForwardMovement = new ForwardModifier(this);
            GravityMovement = new GravityModifier(this);
            TargetMovement = new TargetMovement();
        }

        #region Setter
        public void SetMoveSpeed(float newMoveSpeed)
        {
            _MoveSpeed = newMoveSpeed;
        }
        public void SetAccelateSpeed(float newAccelateSpeed)
        {
            _AcceateSpeed = newAccelateSpeed;
        }
        public void SetDecelerateSpeed(float newDecelerateSpeed)
        {
            _DecelerateSpeed = newDecelerateSpeed;
        }
        public void SetGravity(float newGravity)
        {
            _Gravity = newGravity;
        }
        public void SetUseMovement(bool useDirection)
        {
            _UseMovement = useDirection;

            if (!UseMovement)
                DirectionMovement.ResetVelocity();
                ForwardMovement.ResetVelocity();
        }
        public void SetUseSideStep(bool useSideStep)
        {
            _UseSideStep = useSideStep;
        }
        public void SetUseGravity(bool useGraity)
        {
            _UseGravity = useGraity;

            if(!UseGravity)
                GravityMovement.ResetVelocity();
        }

        public void SetTargetMovement(FTargetMovement targetMovement, float angle)
        {
            TargetMovement.SetTargetMovement(targetMovement, angle);
        }
        public void SetTargetMovementAngle(float angle)
        {
            TargetMovement.SetAngle(angle);
        }
        public void ResetTargetMovement()
        {
            TargetMovement.StopTargetMovement();
        }
        #endregion

        public void OnDirectionMovement(float deltaTime)
        {
            if (!_UseMovement)
                return;

            _Velocity += DirectionMovement.OnMovement(deltaTime);

        }
        public void OnForwardMovement(float deltaTime)
        {
            if (!_UseMovement)
                return;

            _Velocity += ForwardMovement.OnMovement(deltaTime);
        }
        public void OnGravityMovement(float deltaTime)
        {
            if (!_UseGravity)
                return;

            _Velocity += GravityMovement.OnMovement(deltaTime);
        }

        public void OnTargetMovement(float deltaTime)
        {
            if (!TargetMovement.IsActivate)
                return;

            _DeltaVelocity += TargetMovement.OnMovement(deltaTime);
        }

        #region Movement Modifier Class
        protected class GravityModifier : GravityMovement
        {
            public BaseMovementSystem MovementSystem;

            public GravityModifier(BaseMovementSystem movementSystem)
            {
                MovementSystem = movementSystem;
            }

            public void SetMovementSystem(BaseMovementSystem movementSystem)
            {
                MovementSystem = movementSystem;
            }

            public override bool IsGrounded => MovementSystem.IsGrounded;

            public override float CurrentVeritcalSpeed => MovementSystem.CurrentVerticalSpeed;

            public override float Gravity => MovementSystem.GetGravity;
        }

        protected class DirectionalModifier : DirectionMovement
        {
            public BaseMovementSystem MovementSystem;

            public DirectionalModifier(BaseMovementSystem movementSystem)
            {
                MovementSystem = movementSystem;
            }

            public void SetMovementSystem(BaseMovementSystem movementSystem)
            {
                MovementSystem = movementSystem;
            }

            public override float MoveSpeed => MovementSystem.GetMoveSpeed;

            public override float Strength => MovementSystem.MoveStrength;

            public override float AccelateSpeed => MovementSystem.GetAccelateSpeed;

            public override Vector3 MoveDirection => MovementSystem.MoveDirection;

            public override Vector3 LastMoveDirection => MovementSystem.LastMoveDirection;

            public override float DecelerateSpeed => MovementSystem.GetDecelerateSpeed;
        }
        protected class ForwardModifier : ForwardMovement
        {
            public BaseMovementSystem MovementSystem;

            public ForwardModifier(BaseMovementSystem movementSystem)
            {
                MovementSystem = movementSystem;
            }

            public void SetMovementSystem(BaseMovementSystem movementSystem)
            {
                MovementSystem = movementSystem;
            }

            public override float MoveSpeed => MovementSystem.GetMoveSpeed;
            public override float Strength => MovementSystem.MoveStrength;
            public override float AccelateSpeed => MovementSystem.GetAccelateSpeed;
            public override float DecelerateSpeed => MovementSystem.GetDecelerateSpeed;
            public override Transform Transform => MovementSystem.transform;
        }

        #endregion
    }
}
