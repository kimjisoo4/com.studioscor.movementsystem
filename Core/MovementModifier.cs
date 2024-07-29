using UnityEngine;
using StudioScor.Utilities;
namespace StudioScor.MovementSystem
{

    [DefaultExecutionOrder(MovementSystemxcutionOrder.SUB_ORDER)]
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Modifier", order : 10)]
    public abstract class MovementModifierComponent : BaseMonoBehaviour, IMovementModifier
    {
        [Header(" [ Modifier ] ")]
        [SerializeField] private GameObject _owner;
        [SerializeField] protected EMovementUpdateType _updateType = EMovementUpdateType.Default;
        public GameObject Owner => _owner;

        private bool _isPlaying = false;

        private IMovementSystem _movementSystem;
        private IMovementModuleSystem _movementModuleSystem;

        protected IMovementSystem MovementSystem => _movementSystem;
        protected IMovementModuleSystem MovementModuleSystem => _movementModuleSystem;
        public EMovementUpdateType UpdateType => _updateType;
        public bool IsPlaying => _isPlaying;

        protected virtual void Reset()
        {
            if(gameObject.TryGetComponentInParentOrChildren(out IMovementModuleSystem moduleSystem))
            {
                var component = moduleSystem as Component;

                _owner = component.gameObject;
            }
        }

        protected virtual void Awake()
        {
            _movementModuleSystem = _owner.GetMovementModuleSystem();
            _movementSystem = _owner.GetMovementSystem();

            if (_movementModuleSystem is null)
            {
                LogError(" Movement System is NULL");

                return;
            }

            _movementModuleSystem.AddModifier(this);
        }

        private void OnDestroy()
        {
            if (_movementModuleSystem is null)
                return;

            _movementModuleSystem.RemoveModifier(this);
        }

        private void OnEnable()
        {
            EnableModifier();
        }
        private void OnDisable()
        {
            DisableModifier();
        }

        public virtual void EnableModifier()
        {
            ResetModifier();

            _isPlaying = true;
        }
        public virtual void DisableModifier()
        {
            ResetModifier();

            _isPlaying = false;
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