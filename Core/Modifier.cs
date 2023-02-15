using UnityEngine;
using StudioScor.Utilities;
namespace StudioScor.MovementSystem
{

    [DefaultExecutionOrder(MovementSystemxcutionOrder.SUB_ORDER)]
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Modifier", order : 10)]
    public abstract class Modifier : BaseMonoBehaviour, IMovementModifier
    {
        [Header(" [ Modifier ] ")]
        [SerializeField] private MovementSystemComponent _MovementSystemComponent;
        [SerializeField] protected EMovementUpdateType _UpdateType = EMovementUpdateType.Default;
        protected MovementSystemComponent MovementSystem => _MovementSystemComponent;

        public EMovementUpdateType UpdateType => _UpdateType;

        protected virtual void Reset()
        {
            gameObject.TryGetComponentInParentOrChildren(out _MovementSystemComponent);
        }

        private void OnEnable()
        {
            if (!_MovementSystemComponent)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _MovementSystemComponent))
                {
                    Log(" Movement System is NULL", true);
                }
            }

            _MovementSystemComponent.AddModifier(this);

            ResetModifier();
        }
        private void OnDisable()
        {
            if (!_MovementSystemComponent)
                return;

            _MovementSystemComponent.RemoveModifier(this);

            ResetModifier();
        }
        public abstract void ProcessMovement(float deltaTime);
        public virtual void ResetModifier() { }
    }

}