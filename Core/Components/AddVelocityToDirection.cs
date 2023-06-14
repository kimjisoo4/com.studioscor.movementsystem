using UnityEngine;

using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public class AddVelocityToDirection : BaseStateMono
    {
        [Header(" [ AddVelocity To Direction ] ")]
        [SerializeField] private float _Duration = 2f;
        [SerializeField] private EUpdateState _UpdateState = EUpdateState.Update;
        
        [Header(" [ Direction ] ")]
        [SerializeField] private ETransformSpece _TransformSpace = ETransformSpece.Local;
        [SerializeField][SEnumCondition(nameof(_TransformSpace), (int)ETransformSpece.Local, (int)ETransformSpece.World)] private Vector3 _Direction = Vector3.forward;
        [SerializeField][SEnumCondition(nameof(_TransformSpace), (int)ETransformSpece.Local, (int)ETransformSpece.MoveDirection, (int)ETransformSpece.Script)] private bool _UseUpdateDirection;
        
        [Header(" [ Speed ] ")]
        [SerializeField] private float _MaxSpeed = 5f;
        [SerializeField] private float _StartSpeed = 20f;
        [SerializeField] private bool _UseCurve = false;
        [SerializeField][SCondition(nameof(_UseCurve), true, true)] private float _AccelateSpeed = 20f;
        [SerializeField][SCondition(nameof(_UseCurve))] private float _TimeToMaxSpeed = 1f;
        [SerializeField][SCondition(nameof(_UseCurve))] private AnimationCurve _Curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private IMovementSystem _MovementSystem;

        private float _ElapsedTime;
        private float _NormalizedTime;
        private float _CurrentSpeed;

        private Vector3 _MoveDirection;
        private Vector3 _InputDirection;
        private bool _WasInput;

        private void Awake()
        {
            transform.TryGetComponentInParentOrChildren(out _MovementSystem);
        }

        public override bool CanEnterState()
        {
            if (!base.CanEnterState())
                return false;

            if (_MovementSystem is null)
                return false;

            return true;
        }

        public void SetTarget(GameObject target)
        {
            var movementSystem = target.GetComponentInParentOrChildren<IMovementSystem>();

            SetTarget(movementSystem);
        }
        public void SetTarget(Component component)
        {
            var movementSystem = component.GetComponentInParentOrChildren<IMovementSystem>();

            SetTarget(movementSystem);
        }
        public void SetTarget(IMovementSystem movementSystem = null)
        {
            _MovementSystem = movementSystem;
        }

        public void SetDirection(Vector3 direction)
        {
            _WasInput = true;
            _InputDirection = direction;
        }
        public void SetDirectionFromPosition(Vector3 position)
        {
            _WasInput = true;
            _InputDirection = _MovementSystem.transform.Direction(position);
        }

        protected override void EnterState()
        {
            _ElapsedTime = 0f;
            _NormalizedTime = 0f;
            _CurrentSpeed = _StartSpeed;

            CalcDirection();
        }

        protected override void ExitState()
        {
            _WasInput = false;

        }
        private void Update()
        {
            if (_UpdateState != EUpdateState.Update)
                return;

            float deltaTime = Time.deltaTime;

            UpdateState(deltaTime);
        }
        private void FixedUpdate()
        {
            if (_UpdateState != EUpdateState.Fixed)
                return;

            float deltaTime = Time.fixedDeltaTime;

            UpdateState(deltaTime);
        }
        private void LateUpdate()
        {
            if (_UpdateState != EUpdateState.Late)
                return;

            float deltaTime = Time.deltaTime;

            UpdateState(deltaTime);
        }

        public void UpdateState(float deltaTime)
        {
            _ElapsedTime += deltaTime;

            if (_WasInput)
                CalcDirection();

            if (_UseCurve)
                CalcSpeedToCurve();
            else
                CalcSpeedToAccelateSpeed(deltaTime);

            _MovementSystem.AddVelocity(_MoveDirection * _CurrentSpeed);

            if (_Duration <= 0f || _ElapsedTime < _Duration)
                return;

            ForceExitState();
        }


        private void CalcSpeedToCurve()
        {
            if (!_NormalizedTime.SafeEquals(1f))
            {
                _NormalizedTime = Mathf.Min(1f, _ElapsedTime.SafeDivide(_TimeToMaxSpeed));
            }

            float strength = _Curve.Evaluate(_NormalizedTime);

            _CurrentSpeed = Mathf.Lerp(_StartSpeed, _MaxSpeed, strength);
        }
        private void CalcSpeedToAccelateSpeed(float deltaTime)
        {
            if (_CurrentSpeed.SafeEquals(_MaxSpeed))
                return;

            _CurrentSpeed = Mathf.Lerp(_CurrentSpeed, _MaxSpeed, _AccelateSpeed * deltaTime);

            if (_CurrentSpeed.SafeEquals(_MaxSpeed))
                _CurrentSpeed = _MaxSpeed;
        }

        private void CalcDirection()
        {
            switch (_TransformSpace)
            {
                case ETransformSpece.MoveDirection:
                    _MoveDirection = _MovementSystem.MoveDirection;

                    if (_Direction.SafeEquals(Vector3.zero))
                        _MoveDirection = _Direction;
                    break;

                case ETransformSpece.Local:
                    _MoveDirection = _MovementSystem.transform.TransformDirection(_Direction.normalized);
                    break;

                case ETransformSpece.World:
                    _MoveDirection = _Direction.normalized;
                    break;
                case ETransformSpece.Script:
                    _MoveDirection = _InputDirection;
                    break;
            }

            Log($"Direction - X : {_Direction.x} Y : {_Direction.y} Z : {_Direction.z}");
        }
    }
}