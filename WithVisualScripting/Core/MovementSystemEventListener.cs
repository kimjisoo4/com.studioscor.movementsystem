#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using UnityEngine;


namespace StudioScor.MovementSystem.VisualScripting
{
    [DisableAnnotation]
    [AddComponentMenu("")]
    [IncludeInSettings(false)]
    public sealed class MovementSystemEventListener : MessageListener
    {
        private void Awake()
        {
            var movementSystemComponent = GetComponent<MovementSystemComponent>();

            movementSystemComponent.OnLanded += MovementSystemComponent_OnLanded;
            movementSystemComponent.OnJumped += MovementSystemComponent_OnJumped;
            movementSystemComponent.OnStartedMovement += MovementSystemComponent_OnStartedMovement;
            movementSystemComponent.OnFinishedMovement += MovementSystemComponent_OnFinishedMovement;
        }
        private void OnDestroy()
        {
            var movementSystemComponent = GetComponent<MovementSystemComponent>();

            movementSystemComponent.OnLanded -= MovementSystemComponent_OnLanded;
            movementSystemComponent.OnJumped -= MovementSystemComponent_OnJumped;
            movementSystemComponent.OnStartedMovement -= MovementSystemComponent_OnStartedMovement;
            movementSystemComponent.OnFinishedMovement -= MovementSystemComponent_OnFinishedMovement;
        }
        private void MovementSystemComponent_OnFinishedMovement(MovementSystemComponent movementSystem)
        {
            EventBus.Trigger(new EventHook(MovementSystemWithVisualScripting.MOVEMENTSYSTEM_ON_FINISHED_MOVEMENT, movementSystem));
        }

        private void MovementSystemComponent_OnStartedMovement(MovementSystemComponent movementSystem)
        {
            EventBus.Trigger(new EventHook(MovementSystemWithVisualScripting.MOVEMENTSYSTEM_ON_STARTED_MOVEMENT, movementSystem));
        }

        private void MovementSystemComponent_OnJumped(MovementSystemComponent movementSystem)
        {
            EventBus.Trigger(new EventHook(MovementSystemWithVisualScripting.MOVEMENTSYSTEM_ON_JUMPED, movementSystem));
        }

        private void MovementSystemComponent_OnLanded(MovementSystemComponent movementSystem)
        {
            EventBus.Trigger(new EventHook(MovementSystemWithVisualScripting.MOVEMENTSYSTEM_ON_LANDED, movementSystem));
        }
    }
}
#endif