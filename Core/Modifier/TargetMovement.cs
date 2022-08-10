using UnityEngine;
using UnityEngine.Events;


namespace KimScor.MovementSystem
{
    public class TargetMovement : MovementModifier
    {
        #region
        public delegate void TargetMovementHandler(TargetMovement targetMovement);
        #endregion

        [SerializeField] private float _Duration = 0.5f;

        [SerializeField] private bool UseMovementX = false;
        [SerializeField] private float _DistanceX = 5.0f;
        [SerializeField] private AnimationCurve _CurveX = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private bool UseMovementY = false;
        [SerializeField] private float _DistanceY = 5.0f;
        [SerializeField] private AnimationCurve _CurveY = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private bool UseMovementZ = false;
        [SerializeField] private float _DistanceZ = 5.0f;
        [SerializeField] private AnimationCurve _CurveZ = AnimationCurve.Linear(0, 0, 1, 1);

        [SerializeField] private float _CurrentDuration = 0.0f;
        [SerializeField, Range(0f, 1f)] private float _NormalizedTime = 0.0f;

        [SerializeField] private float _PrevDistanceX = 0f;
        [SerializeField] private float _PrevDistanceY = 0f;
        [SerializeField] private float _PrevDistanceZ = 0f;

        [SerializeField] public bool isActivate { get; protected set; }

        public event TargetMovementHandler OnFinishedMovement;

        private Quaternion _Direction;

        public TargetMovement()
        {
            OnFinishedMovement = null;
            isActivate = false;
        }

        public void SetTargetMovement(FTargetMovement curveMove, float angle)
        {
            SetTargetMovement(curveMove);
            SetAngle(angle);
        }
        public void SetTargetMovement(FTargetMovement curveMove)
        {
            if (isActivate)
            {
                OnFinishedMovement?.Invoke(this);
            }

            _Duration = curveMove.Duration;
            UseMovementX = curveMove.UseMovementX;
            _DistanceX = curveMove.DistanceX;
            _CurveX = curveMove.CurveX;
            UseMovementY = curveMove.UseMovementY;
            _DistanceY = curveMove.DistanceY;
            _CurveY = curveMove.CurveY;
            UseMovementZ = curveMove.UseMovementZ;
            _DistanceZ = curveMove.DistanceZ;
            _CurveZ = curveMove.CurveZ;

            _PrevDistanceX = 0f;
            _PrevDistanceY = 0f;
            _PrevDistanceZ = 0f;

            _CurrentDuration = 0.0f;
            _NormalizedTime = 0.0f;

            isActivate = true;
        }

        public void SetAngle(float angle)
        {
            _Direction = Quaternion.Euler(new Vector3(0, angle, 0));
        }

        public void StopTargetMovement()
        {
            if (isActivate)
            {
                OnFinishedMovement?.Invoke(this);

                isActivate = false;
            }
        }

        public Vector3 OnNormalizedMovement(float normalizedTime)
        {
            if (!isActivate)
            {
                return Vector3.zero;
            }

            if (normalizedTime >= 1.0f)
            {
                isActivate = false;

                OnFinishedMovement?.Invoke(this);

                return Vector3.zero;
            }


            Vector3 moveVelocity = Vector3.zero;


            if (UseMovementX)
            {
                float distance = _DistanceX * _CurveX.Evaluate(normalizedTime);

                moveVelocity.x = distance - _PrevDistanceX;

                _PrevDistanceX = distance;
            }

            if (UseMovementY)
            {
                float distance = _DistanceY * _CurveY.Evaluate(normalizedTime);

                moveVelocity.y = distance - _PrevDistanceY;

                _PrevDistanceY = distance;
            }

            if (UseMovementZ)
            {
                float distance = _DistanceZ * _CurveZ.Evaluate(normalizedTime);

                moveVelocity.z = distance - _PrevDistanceZ;

                _PrevDistanceZ = distance;
            }

            return moveVelocity;
        }

        public override void ResetVelocity()
        {
            StopTargetMovement();
        }
        public override Vector3 OnMovement(float deltaTime)
        {
            if (!isActivate)
            {
                return Vector3.zero;
            }

            if (_NormalizedTime >= 1.0f)
            {
                isActivate = false;

                OnFinishedMovement?.Invoke(this);

                return Vector3.zero;
            }

            _CurrentDuration += deltaTime;

            _NormalizedTime = Mathf.Clamp01(_CurrentDuration / _Duration);

            Vector3 moveVelocity = Vector3.zero;


            if (UseMovementX)
            {
                float distance = _DistanceX * _CurveX.Evaluate(_NormalizedTime);

                moveVelocity.x = distance - _PrevDistanceX;

                _PrevDistanceX = distance;
            }

            if (UseMovementY)
            {
                float distance = _DistanceY * _CurveY.Evaluate(_NormalizedTime);

                moveVelocity.y = distance - _PrevDistanceY;

                _PrevDistanceY = distance;
            }

            if (UseMovementZ)
            {
                float distance = _DistanceZ * _CurveZ.Evaluate(_NormalizedTime);

                moveVelocity.z = distance - _PrevDistanceZ;

                _PrevDistanceZ = distance;
            }


            moveVelocity = _Direction * moveVelocity;

            return moveVelocity;
        }

        
    }
}