using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{

    public class MovePositionToCurve : BaseMonoBehaviour
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

        [Header(" [ Play Speed ] ")]
        [SerializeField] private float _PlaySpeed = 1f;

        [Header(" [ Auto Playing ] ")]
        [SerializeField] private bool _AutoPlaying = true;

        private IMovementSystem _MovementSystem;

        private readonly ReachValueToTime _ReachValueToTimeX = new();
        private readonly ReachValueToTime _ReachValueToTimeY = new();
        private readonly ReachValueToTime _ReachValueToTimeZ = new();

        private float _Strength = 1f;
        private float _NormalizedTime = 0f;
        private float _ElapsedTime = 0f;

        private Quaternion _Rotate;
        private bool _WasInput = false;
        private Vector3 _InputDirection = default;

        private bool _IsPlaying = false;
        

        public float NormalizedTime => _NormalizedTime;

        public void SetPlaySpeed(float newSpeed)
        {
            _PlaySpeed = newSpeed;
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

        private void OnEnable()
        {
            if (_AutoPlaying)
                OnMovement();
        }
        private void OnDisable()
        {
            EndMovement();
        }
        
        private void Update()
        {
            if (_UpdateState != EUpdateState.Update)
                return;

            float deltaTime = Time.deltaTime * _PlaySpeed;

            UpdateMovement(deltaTime);
        }
        private void FixedUpdate()
        {
            if (_UpdateState != EUpdateState.Fixed)
                return;

            float deltaTime = Time.fixedDeltaTime * _PlaySpeed;

            UpdateMovement(deltaTime);
        }
        private void LateUpdate()
        {
            if (_UpdateState != EUpdateState.Late)
                return;

            float deltaTime = Time.deltaTime * _PlaySpeed;

            UpdateMovement(deltaTime);
        }

        public void OnMovement()
        {
            if (_IsPlaying)
                return;

            _IsPlaying = true;

            _NormalizedTime = 0f;
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
        public void EndMovement()
        {
            if (!_IsPlaying)
                return;

            _IsPlaying = false;

            _WasInput = false;

            _ReachValueToTimeX.EndMovement();
            _ReachValueToTimeY.EndMovement();
            _ReachValueToTimeZ.EndMovement();
        }

        public void UpdateMovementToNormalizedTime(float normalizedTime)
        {
            if (!_IsPlaying)
                return;

            _NormalizedTime = normalizedTime;

            if (_UseUpdateDirection)
                CalcRotate();

            Movement();
        }
        public void UpdateMovement(float deltaTime)
        {
            if (!_IsPlaying)
                return;

            CalcNormalizedTime(deltaTime);

            if (_UseUpdateDirection)
                CalcRotate();

            Movement();
        }

        private void Movement()
        {
            float x = _ReachValueToTimeX.UpdateMovement(_NormalizedTime);
            float y = _ReachValueToTimeY.UpdateMovement(_NormalizedTime);
            float z = _ReachValueToTimeZ.UpdateMovement(_NormalizedTime);

            Vector3 addPosition = new Vector3(x, y, z);

            _MovementSystem.MovePosition(_Rotate * addPosition);

            if (_NormalizedTime < 1f)
                return;

            EndMovement();
        }


        private void CalcNormalizedTime(float deltaTime)
        {
            _ElapsedTime += deltaTime;
            _NormalizedTime = _ElapsedTime.SafeDivide(_Duration);

            _NormalizedTime = Mathf.Min(1f, _NormalizedTime);
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

        private void CalcRotateToLocal()
        {
            Vector3 direction = _MovementSystem.transform.TransformDirection(_Direction);

            _Rotate = Quaternion.LookRotation(direction);
        }
        private void CalcRotateToMoveDirection()
        {
            if (_MovementSystem.MoveDirection.SafeEqauls(Vector3.zero))
            {
                CalcRotateToLocal();

                return;
            }

            _Rotate = Quaternion.LookRotation(_MovementSystem.MoveDirection);
        }

        private void CalcRotateToWolrd()
        {
            if (_Direction.SafeEqauls(Vector3.zero))
            {
                CalcRotateToLocal();

                return;
            }

            _Rotate = Quaternion.LookRotation(_Direction);
        }
        private void CalcRotateToScript()
        {
            if (_InputDirection.SafeEqauls(Vector3.zero))
            {
                CalcRotateToLocal();

                return;
            }
            
            _Rotate = Quaternion.LookRotation(_InputDirection);
        }
    }
}