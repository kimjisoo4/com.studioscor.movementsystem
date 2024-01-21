using UnityEngine;
using StudioScor.Utilities;
namespace StudioScor.MovementSystem
{

    [DefaultExecutionOrder(MovementSystemxcutionOrder.SUB_ORDER)]
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Modifier", order : 10)]
    public abstract class MovementModifierComponent : BaseMonoBehaviour, IMovementModifier
    {
        [Header(" [ Modifier ] ")]
        [SerializeField] private GameObject _Owner;
        [SerializeField] protected EMovementUpdateType _UpdateType = EMovementUpdateType.Default;
        public GameObject Owner => _Owner;

        private bool isPlaying = false;

        private IMovementSystem _MovementSystem;
        private IMovementModuleSystem _MovementModuleSystem;

        protected IMovementSystem MovementSystem => _MovementSystem;
        protected IMovementModuleSystem MovementModuleSystem => _MovementModuleSystem;
        public EMovementUpdateType UpdateType => _UpdateType;
        public bool IsPlaying => isPlaying;

        protected virtual void Reset()
        {
            if(gameObject.TryGetComponentInParentOrChildren(out IMovementModuleSystem moduleSystem))
            {
                var component = moduleSystem as Component;

                _Owner = component.gameObject;
            }
        }

        protected virtual void Awake()
        {
            _MovementModuleSystem = _Owner.GetMovementModuleSystem();
            _MovementSystem = _Owner.GetMovementSystem();

            if (_MovementModuleSystem is null)
            {
                LogError(" Movement System is NULL");

                return;
            }

            _MovementModuleSystem.AddModifier(this);
        }

        private void OnDestroy()
        {
            if (_MovementModuleSystem is null)
                return;

            _MovementModuleSystem.RemoveModifier(this);
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

            isPlaying = true;
        }
        public virtual void DisableModifier()
        {
            ResetModifier();

            isPlaying = false;
        }

        public void ProcessMovement(float deltaTime)
        {
            if (!isPlaying)
                return;

            UpdateMovement(deltaTime);
        }

        protected abstract void UpdateMovement(float deltaTime);
        public virtual void ResetModifier() { }
    }

}