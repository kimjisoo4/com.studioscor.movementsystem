using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public class SingleMovePositionToCurve : BaseMonoBehaviour
    {
        [Header(" [ Add Position To Curve ] ")]
        [SerializeField] private float moveDuration = 0.5f;
        [SerializeField] private EUpdateState updateState = EUpdateState.Update;

        [Header(" [ Direction ] ")]
        [SerializeField] private ETransformSpece moveSpace = ETransformSpece.Local;
        [SerializeField] private Vector3 moveDirection = Vector3.forward;
        [SerializeField] private bool useUpdateDirection;

        [Header(" [ Movement ] ")]
        [SerializeField] private float moveDistance = 0f;
        [SerializeField] private bool useScaleDistance = true;
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        [Header(" [ Play Speed ] ")]
        [SerializeField] private float playSpeed = 1f;

        [Header(" [ Auto Playing ] ")]
        [SerializeField] private bool useAutoPlaying = true;

        private IMovementSystem movementSystem;

        private readonly ReachValueToTime reachValueToTime = new();

        private float moveStrength = 1f;
        private float normalizedTime = 0f;
        private float elapsedTime = 0f;

        private Quaternion _Rotate;
        private bool wasInput = false;
        private Vector3 inputDirection = default;

        private bool isPlaying = false;

        public float NormalizedTime => normalizedTime;

        public void SetPlaySpeed(float newSpeed)
        {
            playSpeed = newSpeed;
        }
        public void SetStrength(float strength)
        {
            moveStrength = strength;
        }
        public void SetDirection(Vector3 direction)
        {
            wasInput = true;

            inputDirection = direction;
        }
        public void SetDistance(float distance)
        {
            moveDistance = distance;
        }
        public void SetDuration(float duration)
        {
            moveDuration = duration;
        }
        public void SetCurve(AnimationCurve curve)
        {
            moveCurve = curve;
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
            this.movementSystem = movementSystem;
        }

        private void Awake()
        {
            if(movementSystem is null)
                transform.TryGetComponentInParentOrChildren(out movementSystem);
        }

        private void OnEnable()
        {
            if (useAutoPlaying)
                OnMovement();
        }
        private void OnDisable()
        {
            EndMovement();
        }

        private void Update()
        {
            if (updateState != EUpdateState.Update)
                return;

            float deltaTime = Time.deltaTime * playSpeed;

            UpdateMovement(deltaTime);
        }
        private void FixedUpdate()
        {
            if (updateState != EUpdateState.Fixed)
                return;

            float deltaTime = Time.fixedDeltaTime * playSpeed;

            UpdateMovement(deltaTime);
        }
        private void LateUpdate()
        {
            if (updateState != EUpdateState.Late)
                return;

            float deltaTime = Time.deltaTime * playSpeed;

            UpdateMovement(deltaTime);
        }

        public void OnMovement()
        {
            if (isPlaying)
                return;

            isPlaying = true;

            normalizedTime = 0f;
            elapsedTime = 0f;

            CalcRotate();

            float distance = useScaleDistance ? moveDistance * moveStrength : moveDistance;

            reachValueToTime.OnMovement(distance, moveCurve);
        }
        public void EndMovement()
        {
            if (!isPlaying)
                return;

            isPlaying = false;

            wasInput = false;

            reachValueToTime.EndMovement();
        }

        public void UpdateMovementToNormalizedTime(float normalizedTime)
        {
            if (!isPlaying)
                return;

            this.normalizedTime = normalizedTime;

            if (useUpdateDirection)
                CalcRotate();

            Movement();
        }
        public void UpdateMovement(float deltaTime)
        {
            if (!isPlaying)
                return;

            CalcNormalizedTime(deltaTime);

            if (useUpdateDirection)
                CalcRotate();

            Movement();
        }

        private void Movement()
        {
            float distance = reachValueToTime.UpdateMovement(normalizedTime);

            Vector3 addPosition = new Vector3(0, 0, distance);

            movementSystem.MovePosition(_Rotate * addPosition);

            if (normalizedTime < 1f)
                return;

            EndMovement();
        }


        private void CalcNormalizedTime(float deltaTime)
        {
            elapsedTime += deltaTime;
            normalizedTime = elapsedTime.SafeDivide(moveDuration);

            normalizedTime = Mathf.Min(1f, normalizedTime);
        }

        private void CalcRotate()
        {
            switch (moveSpace)
            {
                case ETransformSpece.MoveDirection:
                    if (movementSystem.MoveStrength > 0f)
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
                    if (wasInput)
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
            Vector3 direction = movementSystem.transform.TransformDirection(moveDirection);

            _Rotate = Quaternion.LookRotation(direction);
        }
        private void CalcRotateToMoveDirection()
        {
            if (movementSystem.MoveDirection.SafeEqauls(Vector3.zero))
            {
                CalcRotateToLocal();

                return;
            }

            _Rotate = Quaternion.LookRotation(movementSystem.MoveDirection);
        }

        private void CalcRotateToWolrd()
        {
            if (moveDirection.SafeEqauls(Vector3.zero))
            {
                CalcRotateToLocal();

                return;
            }

            _Rotate = Quaternion.LookRotation(moveDirection);
        }
        private void CalcRotateToScript()
        {
            if (inputDirection.SafeEqauls(Vector3.zero))
            {
                CalcRotateToLocal();

                return;
            }

            _Rotate = Quaternion.LookRotation(inputDirection);
        }
    }
}