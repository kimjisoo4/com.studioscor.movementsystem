using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    [System.Serializable]
    public abstract class MovementModifier : BaseClass, IMovementModifier
    {
        [Header(" [ Movement Modifier ] ")]
        private EMovementUpdateType _updateType = EMovementUpdateType.Default;
        private IMovementSystem _movementSystem;
        private IMovementModuleSystem _moduleSystem;

        private bool _isPlaying = false;
        public EMovementUpdateType UpdateType => _updateType;
        public bool IsPlaying => _isPlaying;
        
        public IMovementSystem MovementSystem => _movementSystem;
        public IMovementModuleSystem ModuleSystem => _moduleSystem;

#if UNITY_EDITOR
        public override Object Context => _movementSystem.gameObject;
#endif

        public MovementModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem, EMovementUpdateType updateType = EMovementUpdateType.Default)
        {
            SetupModifier(movementSystem, moduleSystem, updateType);
        }

        public void SetupModifier(IMovementSystem movementSystem, IMovementModuleSystem moduleSystem, EMovementUpdateType updateType = EMovementUpdateType.Default)
        {
            _movementSystem = movementSystem;
            _moduleSystem = moduleSystem;
            _updateType = updateType;

            moduleSystem.AddModifier(this);
        }

        public void EnableModifier()
        {
            _isPlaying = true;

            ResetModifier();
        }
        public void DisableModifier()
        {
            _isPlaying = false;

            ResetModifier();
        }

        public void ProcessMovement(float deltaTime)
        {
            if (!_isPlaying)
                return;

            UpdateMovement(deltaTime);
        }

        protected abstract void UpdateMovement(float deltaTime);
        public virtual void ResetModifier() { }
    }

}