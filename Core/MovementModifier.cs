using UnityEngine;
using StudioScor.Utilities;
namespace StudioScor.MovementSystem
{

    [DefaultExecutionOrder(MovementSystemxcutionOrder.SUB_ORDER)]
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Modifier", order : 10)]
    public abstract class MovementModifierComponent : BaseMonoBehaviour, IMovementModifier
    {
        [Header(" [ Modifier ] ")]
        [SerializeField] private GameObject owner;
        [SerializeField] protected EMovementUpdateType _UpdateType = EMovementUpdateType.Default;
        public GameObject Owner => owner;

        private bool isPlaying = false;

        private IMovementSystem movementSystem;
        private IMovementModuleSystem movementModuleSystem;

        protected IMovementSystem MovementSystem => movementSystem;
        protected IMovementModuleSystem MovementModuleSystem => movementModuleSystem;
        public EMovementUpdateType UpdateType => _UpdateType;
        public bool IsPlaying => isPlaying;

        protected virtual void Reset()
        {
            if(gameObject.TryGetComponentInParentOrChildren(out IMovementModuleSystem moduleSystem))
            {
                var component = moduleSystem as Component;

                owner = component.gameObject;
            }
        }

        protected virtual void Awake()
        {
            movementModuleSystem = owner.GetMovementModuleSystem();
            movementSystem = owner.GetMovementSystem();

            if (movementModuleSystem is null)
            {
                LogError(" Movement System is NULL");

                return;
            }

            movementModuleSystem.AddModifier(this);
        }

        private void OnDestroy()
        {
            if (movementModuleSystem is null)
                return;

            movementModuleSystem.RemoveModifier(this);
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