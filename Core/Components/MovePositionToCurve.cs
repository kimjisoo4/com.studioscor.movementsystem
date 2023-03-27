using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public class MovePositionToCurve : BaseStateMono
    {
        [Header(" [ Add Position To Curve ] ")]
        [SerializeField] private float _Duration = 0.5f;
        [SerializeField] private EUpdateState _UpdateState = EUpdateState.Update;

        [Header(" [ Direction ] ")]
        [SerializeField] private ETransformSpece _Space = ETransformSpece.Local;
        [SerializeField] private Vector3 _Direction = Vector3.forward;
        [SerializeField] private bool _UseUpdateDirection;

        [Header(" [ Movement ] ")]
        [SerializeField] private bool _UseX = false;
        [SerializeField, SCondition(nameof(_UseX), true)] private bool _UseScaleXToStrength = true;
        [SerializeField, SCondition(nameof(_UseX), true)] private float _DistanceX = 0f;
        [SerializeField, SCondition(nameof(_UseX), true)] private AnimationCurve _CurveX = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [SerializeField] private bool _UseY = false;
        [SerializeField, SCondition(nameof(_UseY), true)] private bool _UseScaleYToStrength = false;
        [SerializeField, SCondition(nameof(_UseY), true)] private float _DistanceY = 0f;
        [SerializeField, SCondition(nameof(_UseY), true)] private AnimationCurve _CurveY = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [SerializeField] private bool _UseZ = true;
        [SerializeField, SCondition(nameof(_UseZ), true)] private bool _UseScaleZToStrength = true;
        [SerializeField, SCondition(nameof(_UseZ), true)] private float _DistanceZ = 5f;
        [SerializeField, SCondition(nameof(_UseZ), true)] private AnimationCurve _CurveZ = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private IMovementSystem _MovementSystem;

        private readonly ReachValueToTime _ReachValueToTimeX = new();
        private readonly ReachValueToTime _ReachValueToTimeY = new();
        private readonly ReachValueToTime _ReachValueToTimeZ = new();

        private float _Strength = 1f;
        private float _Progress = 0f;
        private float _ElapsedTime = 0f;

        private Quaternion _Rotate;
        private bool _WasInput = false;
        private Vector3 _InputDirection = default;

        public override bool CanEnterState()
        {
            if (!base.CanEnterState())
                return false;

            if (_MovementSystem is null)
                return false;

            if (_Duration <= 0f)
                return false;

            return true;
        }

        public void SetStrength(float strength)
        {
            _Strength = strength;
        }
        public void SetDirection(Vector3 direction)
        {
            _WasInput = true;

            _InputDirection = direction;
        }
        public void SetTarget(Component component)
        {
            IMovementSystem movementSystem = component.GetComponentInParentOrChildren<IMovementSystem>();

            SetTarget(movementSystem);
        }
        public void SetTarget(GameObject target)
        {
            IMovementSystem movementSystem = target.GetComponentInParentOrChildren<IMovementSystem>();
            
            SetTarget(movementSystem);
        }
        public void SetTarget(IMovementSystem movementSystem = null)
        {
            _MovementSystem = movementSystem;
        }

        private void Awake()
        {
            transform.TryGetComponentInParentOrChildren(out _MovementSystem);
        }

        protected override void EnterState()
        {
            _Progress = 0f;
            _ElapsedTime = 0f;

            CalcRotate();

            if (_UseX)
            {
                float distance = _UseScaleXToStrength ? _DistanceX * _Strength : _DistanceX;

                _ReachValueToTimeX.OnMovement(distance, _CurveX);
            }
            if (_UseY)
            {
                float distance = _UseScaleYToStrength ? _DistanceY * _Strength : _DistanceY;

                _ReachValueToTimeY.OnMovement(distance, _CurveY);
            }
            if (_UseZ)
            {
                float distance = _UseScaleZToStrength ? _DistanceZ * _Strength : _DistanceZ;

                _ReachValueToTimeZ.OnMovement(distance, _CurveZ);
            }
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
            _Progress = _ElapsedTime.SafeDivide(_Duration);

            _Progress = Mathf.Min(1f, _Progress);

            if (_UseUpdateDirection)
                CalcRotate();

            float x = _ReachValueToTimeX.UpdateMovement(_Progress);
            float y = _ReachValueToTimeY.UpdateMovement(_Progress);
            float z = _ReachValueToTimeZ.UpdateMovement(_Progress);

            Vector3 addPosition = new Vector3(x, y, z);

            _MovementSystem.MovePosition(_Rotate * addPosition);

            if (_Progress < 1f)
                return;

            ForceExitState();
        }


        private void CalcRotate()
        {
            switch (_Space)
            {
                case ETransformSpece.MoveDirection:
                    if (_MovementSystem.MoveStrength > 0f)
                    {
                        CalcRotateToMoveDirection();
                    }
                    else
                    {
                        CalcRotateToLocal();
                    }
                    break;
                case ETransformSpece.Local:
                    CalcRotateToLocal();
                    break;
                case ETransformSpece.World:
                    CalcRotateToWolrd();
                    break;
                case ETransformSpece.Script:
                    if (_WasInput)
                    {
                        CalcRotateToScript();
                    }
                    else
                    {
                        CalcRotateToLocal();
                    }
                    break;
                default:
                    _Rotate = Quaternion.identity;
                    break;
            }
        }

        private void CalcRotateToMoveDirection()
        {
            _Rotate = Quaternion.LookRotation(_MovementSystem.MoveDirection, _MovementSystem.transform.up);
        }
        private void CalcRotateToLocal()
        {
            Vector3 direction = _MovementSystem.transform.TransformDirection(_Direction);

            _Rotate = Quaternion.LookRotation(direction);
        }
        private void CalcRotateToWolrd()
        {
            _Rotate = Quaternion.LookRotation(_Direction);
        }
        private void CalcRotateToScript()
        {
            _Rotate = Quaternion.LookRotation(_InputDirection);
        }
    }
}