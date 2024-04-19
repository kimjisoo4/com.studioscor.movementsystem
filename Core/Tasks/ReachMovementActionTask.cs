using StudioScor.Utilities;
using System;
using UnityEngine;

namespace StudioScor.MovementSystem
{
    [Serializable]
    public class ReachMovementActionTask : ActionTask, ISubActionTask
    {
        [Header(" [ Reach Movement Ability Task ] ")]
        [SerializeReference]
#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReferenceDropdown]
#endif
        private IDirectionVariable _direction = new LocalDirectionVariable(Vector3.forward);

        [SerializeReference]
#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReferenceDropdown]
#endif
        private IFloatVariable _distance = new DefaultFloatVariable(5f);
        [SerializeField] private AnimationCurve _curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

        private Vector3 Direction => _direction.GetValue();
        private float Distance => _original is not null ? _original._distance.GetValue() : _distance.GetValue();
        private AnimationCurve Curve => _original is null ? _curve : _original._curve;

        public bool IsFixedUpdate => false;

        private Vector3 _moveDirection;

        private IMovementSystem _movementSystem;
        private readonly ReachValueToTime _reachValueToTime = new();
        private ReachMovementActionTask _original;

        public override IActionTask Clone()
        {
            var copy = new ReachMovementActionTask();

            copy._original = this;
            copy._direction = _direction.Clone() as IDirectionVariable;

            return copy;
        }
        protected override void SetupTask()
        {
            base.SetupTask();

            _movementSystem = Owner.GetMovementSystem();

            _direction.Setup(Owner);
        }
        protected override void EnterTask()
        {
            base.EnterTask();

            _moveDirection = Direction;
            _reachValueToTime.OnMovement(Distance, Curve);
        }

        protected override void ExitTask()
        {
            base.ExitTask();

            _reachValueToTime.EndMovement();
        }
        public void UpdateSubTask(float normalizedTime)
        {
            float speed = _reachValueToTime.UpdateMovement(normalizedTime);

            _movementSystem.MovePosition(_moveDirection * speed);
        }
    }

}
