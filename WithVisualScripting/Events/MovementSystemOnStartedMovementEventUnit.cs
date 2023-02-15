#if SCOR_ENABLE_VISUALSCRIPTING
using System;
using Unity.VisualScripting;


namespace StudioScor.MovementSystem.VisualScripting
{
    [UnitTitle("On Started Movement")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\StudioScor\\MovementSystem")]
    public class MovementSystemOnStartedMovementEventUnit : MovementSystemEventUnit
    {
        protected override string HookName => MovementSystemWithVisualScripting.MOVEMENTSYSTEM_ON_STARTED_MOVEMENT;
    }
}
#endif