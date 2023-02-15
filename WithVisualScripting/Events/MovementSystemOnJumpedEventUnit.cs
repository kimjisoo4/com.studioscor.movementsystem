#if SCOR_ENABLE_VISUALSCRIPTING
using System;
using Unity.VisualScripting;

namespace StudioScor.MovementSystem.VisualScripting
{
    [UnitTitle("On Jumped")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\StudioScor\\MovementSystem")]
    public class MovementSystemOnJumpedEventUnit : MovementSystemEventUnit
    {
        protected override string HookName => MovementSystemWithVisualScripting.MOVEMENTSYSTEM_ON_JUMPED;
    }
}

#endif