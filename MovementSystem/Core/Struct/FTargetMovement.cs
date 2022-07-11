using UnityEngine;

namespace KimScor.MovementSystem
{
    [System.Serializable]
    public struct FTargetMovement
    {
        [Header(" [ 이동 시간 ] ")]
        [SerializeField] public float Duration;

        [Header("[ Setting ]")]
        public bool UseMovementX;
        public float DistanceX;
        public AnimationCurve CurveX;

        [Space(5)]
        public bool UseMovementY;
        public float DistanceY;
        public AnimationCurve CurveY;

        [Space(5)]
        public bool UseMovementZ;
        public float DistanceZ;
        public AnimationCurve CurveZ;

        public FTargetMovement(float duration,
                                       bool useMovementX, float distanceX, AnimationCurve curveX,
                                       bool useMovementY, float distanceY, AnimationCurve curveY,
                                       bool useMovementZ, float distanceZ, AnimationCurve curveZ)
        {
            Duration = duration;
            UseMovementX = useMovementX;
            DistanceX = distanceX;
            CurveX = curveX;
            UseMovementY = useMovementY;
            DistanceY = distanceY;
            CurveY = curveY;
            UseMovementZ = useMovementZ;
            DistanceZ = distanceZ;
            CurveZ = curveZ;
        }
    }

}