using StudioScor.Utilities;
using System;
using UnityEngine;

namespace StudioScor.MovementSystem
{
    [Serializable]
    public class TeleportMoveActionTask : Task, ISubTask
    {
        [Header(" [ Telport Action Task ] ")]
#if SCOR_ENABLE_SERIALIZEREFERENCE
        [SerializeReference, SerializeReferenceDropdown]
#endif
        private IPositionVariable _teleportPosition;

        [SerializeField] private float _startTime = 0.2f;

        private float _start;
        private IMovementSystem _movementSystem;
        private TeleportMoveActionTask _origjnal;

        protected override void SetupTask()
        {
            base.SetupTask();

            _teleportPosition.Setup(Owner);

            _movementSystem = Owner.GetMovementSystem();
        }

        public override ITask Clone()
        {
            var clone = new TeleportMoveActionTask();

            clone._origjnal = this;
            clone._teleportPosition = _teleportPosition.Clone();

            return clone;
        }
        protected override void EnterTask()
        {
            base.EnterTask();
            
            _start = _origjnal is null ? _startTime : _origjnal._startTime;
        }
        public void FixedUpdateSubTask(float deltaTime, float normalizedTime)
        {
            if (!IsPlaying)
                return;

            if(_start <= normalizedTime)
            {
                _movementSystem.Teleport(_teleportPosition.GetValue());

                ComplateTask();
            }
        }

        public void UpdateSubTask(float deltaTime, float normalizedTime)
        {
            return;
        }
    }

}
