using UnityEngine;
using StudioScor.Utilities;
namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/RootMotion Modifier", order: 30)]
    public class RootMotionModifier : MovementModifier
    {
        [Header(" [ Root Motion Modifier ] ")]
        [SerializeField] private Animator _Animator;
        private Vector3 _RootPosition = Vector3.zero;

        protected override void Reset()
        {
            base.Reset();

            gameObject.TryGetComponentInParentOrChildren(out _Animator);
        }

        private void Awake()
        {
            if(!_Animator)
            {
                if(!gameObject.TryGetComponentInParentOrChildren(out _Animator))
                {
                    Log("Animator Is NULL!!", true);
                }
            }
        }

        public override void ProcessMovement(float deltaTime)
        {
            MovementSystem.MovePosition(_RootPosition);
        }

        private void OnAnimatorMove()
        {
            _RootPosition = _Animator.deltaPosition;
        }
    }

}