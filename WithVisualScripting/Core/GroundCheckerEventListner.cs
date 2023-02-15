#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using UnityEngine;


namespace StudioScor.MovementSystem.VisualScripting
{
    [DisableAnnotation]
    [AddComponentMenu("")]
    [IncludeInSettings(false)]
    public sealed class GroundCheckerEventListner : MessageListener
    {
        private void Awake()
        {
            var groundChecker = GetComponent<GroundChecker>();

            groundChecker.OnChangedGroundedState += GroundChecker_OnChangedGroundedState;
        }
        private void OnDestroy()
        {
            if (TryGetComponent(out GroundChecker groundChecker))
            {
                groundChecker.OnChangedGroundedState -= GroundChecker_OnChangedGroundedState;
            }
        }

        private void GroundChecker_OnChangedGroundedState(GroundChecker groundChecker, bool isGrounded)
        {
            EventBus.Trigger(new EventHook(MovementSystemWithVisualScripting.GROUNDCHECKER_ON_CHANGED_GROUNDED_STATE, groundChecker), isGrounded);
        }
    }
}

#endif