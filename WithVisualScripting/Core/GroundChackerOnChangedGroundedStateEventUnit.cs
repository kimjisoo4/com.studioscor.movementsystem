#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using System;
using StudioScor.Utilities.VisualScripting;

namespace StudioScor.MovementSystem.VisualScripting
{
    [UnitTitle("On GroundChecker Changed Grounded State")]
    [UnitShortTitle("OnChangedGroundedState")]
    [UnitCategory("Events\\StudioScor\\MovementSystem\\GroundChecker")]
    public class GroundChackerOnChangedGroundedStateEventUnit : CustomEventUnit<GroundChecker, bool>
    {
        [DoNotSerialize]
        [PortLabel("IsGrounded")]
        public ValueOutput IsGrounded { get; private set; }
        public override Type MessageListenerType => typeof(GroundCheckerEventListner);

        protected override string HookName => MovementSystemWithVisualScripting.GROUNDCHECKER_ON_CHANGED_GROUNDED_STATE;

        protected override void Definition()
        {
            base.Definition();

            IsGrounded = ValueOutput<bool>(nameof(IsGrounded));
        }
        protected override void AssignArguments(Flow flow, bool value)
        {
            base.AssignArguments(flow, value);

            flow.SetValue(IsGrounded, value);
        }


    }
}

#endif