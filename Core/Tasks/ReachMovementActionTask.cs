using StudioScor.Utilities;
using System;
using UnityEngine;

namespace StudioScor.MovementSystem
{
    [Serializable]
    public class ReachMovementActionTask : Task, ISubTask
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
        [SerializeField] private bool _updatableDirection = false;
        public bool IsFixedUpdate => false;

        private Vector3 _moveDirection;
        private float _moveDistance;
        private bool _updatable;
        private AnimationCurve _animationCurve;

        private IMovementSystem _movementSystem;
        private readonly ReachValueToTime _reachValueToTime = new();
        private ReachMovementActionTask _original;

        public override ITask Clone()
        {
            var clone = new ReachMovementActionTask();

            clone._original = this;
            clone._direction = _direction.Clone();
            clone._distance = _distance.Clone();

            return clone;
        }
        protected override void SetupTask()
        {
            base.SetupTask();

            _movementSystem = Owner.GetMovementSystem();

            _direction.Setup(Owner);
            _distance.Setup(Owner);
        }
        protected override void EnterTask()
        {
            base.EnterTask();

            _moveDirection = _direction.GetValue();
            _moveDistance = _distance.GetValue();
            _animationCurve = _original is null ? _curve : _original._curve;
            _updatable = _original is null ? _updatableDirection : _original._updatableDirection;

            _reachValueToTime.OnMovement(_moveDistance, _animationCurve);
        }

        protected override void ExitTask()
        {
            base.ExitTask();

            _reachValueToTime.EndMovement();
        }
        public void UpdateSubTask(float normalizedTime)
        {
            if (_updatable)
            {
                _moveDirection = _direction.GetValue();
            }

            float speed = _reachValueToTime.UpdateMovement(normalizedTime);

            _movementSystem.MovePosition(_moveDirection * speed);
        }
        public void FixedUpdateSubTask(float normalizedTime)
        {
            return;
        }
    }

}
