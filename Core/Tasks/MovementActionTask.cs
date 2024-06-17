using StudioScor.Utilities;
using System;
using UnityEngine;

namespace StudioScor.MovementSystem
{
    [Serializable]
    public class MovementActionTask : Task, ISubTask
    {
        [Header(" [ Movement Action Task ] ")]
#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReference, SerializeReferenceDropdown]
#endif
        private IDirectionVariable _direction = new LocalDirectionVariable(Vector3.forward);

#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReference, SerializeReferenceDropdown]
#endif
        private IFloatVariable _startSpeed = new DefaultFloatVariable(20f);

#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReference, SerializeReferenceDropdown]
#endif
        private IFloatVariable _targetSpeed = new DefaultFloatVariable(5f);


#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReference, SerializeReferenceDropdown]
#endif
        private IFloatVariable _accelerateSpeed = new DefaultFloatVariable(20f);


#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReference, SerializeReferenceDropdown]
#endif
        private IFloatVariable _decelerateSpeed = new DefaultFloatVariable(10f);

        [SerializeField] private bool _updatableDirection = false;
        [SerializeField] private bool _usePhysics = false;

        private AccelerateMove _accelerateMove;
        private MovementActionTask _original;
        private IMovementSystem _movementSystem;


        private Vector3 _moveDirection;
        private bool _physics;
        private bool _updatable;

        protected override void SetupTask()
        {
            base.SetupTask();

            _movementSystem = Owner.GetMovementSystem();

            if(_accelerateMove is null)
            {
                _accelerateMove = new();
            }

            _direction.Setup(Owner);
            _targetSpeed.Setup(Owner);
            _startSpeed.Setup(Owner);
            _accelerateSpeed.Setup(Owner);
            _decelerateSpeed.Setup(Owner);
        }

        public override ITask Clone()
        {
            var clone = new MovementActionTask();

            clone._original = this;
            clone._direction = _direction.Clone();
            clone._targetSpeed = _targetSpeed.Clone();
            clone._startSpeed = _startSpeed.Clone();
            clone._accelerateSpeed = _accelerateSpeed.Clone();
            clone._decelerateSpeed = _decelerateSpeed.Clone();

            return clone;
        }

        protected override void EnterTask()
        {
            base.EnterTask();

            var isOriginal = _original is null;

            _accelerateMove.StartSpeed = _startSpeed.GetValue();
            _accelerateMove.TargetSpeed = _targetSpeed.GetValue();
            _accelerateMove.AccelerateSpeed = _accelerateSpeed.GetValue();
            _accelerateMove.DecelerateSpeed = _decelerateSpeed.GetValue();

            _moveDirection = _direction.GetValue();
            _physics = isOriginal ? _usePhysics : _original._physics;
            _updatable = isOriginal ? _updatableDirection : _original._updatableDirection;

            _accelerateMove.OnAccelerate();
        }

        protected override void ExitTask()
        {
            base.ExitTask();

            _accelerateMove.EndAccelerate();
        }

        public void FixedUpdateSubTask(float deltaTime, float normalizedTime)
        {
            if (!_physics)
                return;

            UpdateMovement(deltaTime);
        }

        public void UpdateSubTask(float deltaTime, float normalizedTime)
        {
            if (_physics)
                return;

            UpdateMovement(deltaTime);
        }

        private void UpdateMovement(float deltaTime)
        {
            if(_updatable)
            {
                _moveDirection = _direction.GetValue();
            }

            _accelerateMove.UpdateAccelerate(deltaTime);

            _movementSystem.AddVelocity(_moveDirection * _accelerateMove.Speed);
        }
    }

}
