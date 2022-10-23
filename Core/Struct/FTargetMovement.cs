using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace KimScor.MovementSystem
{
    [System.Serializable]
    public struct FSingleTargetMovement
    {
        public FSingleTargetMovement(float duration, float distance, AnimationCurve curve)
        {
            Duration = duration;
            Distance = distance;
            Curve = curve;
        }

        public float Duration;
        public float Distance;
        public AnimationCurve Curve;
    }

    [System.Serializable]
    public struct FTargetMovement
    {
        [Header(" [ 이동 시간 ] ")]
        [SerializeField] public float Duration;

        [Header("[ Setting ]")]
        public bool UseMovementX;
#if ODIN_INSPECTOR
        [ShowIf("UseMovementX")]
#endif
        public float DistanceX;
#if ODIN_INSPECTOR
        [ShowIf("UseMovementX")]
#endif
        public AnimationCurve CurveX;

        [Space(5)]
        public bool UseMovementY;
#if ODIN_INSPECTOR
        [ShowIf("UseMovementY")]
#endif
        public float DistanceY;
#if ODIN_INSPECTOR
        [ShowIf("UseMovementY")]
#endif
        public AnimationCurve CurveY;

        [Space(5)]
        public bool UseMovementZ;
#if ODIN_INSPECTOR
        [ShowIf("UseMovementZ")]
#endif
        public float DistanceZ;
#if ODIN_INSPECTOR
        [ShowIf("UseMovementZ")]
#endif
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