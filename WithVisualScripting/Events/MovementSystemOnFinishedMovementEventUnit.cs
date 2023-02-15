#if SCOR_ENABLE_VISUALSCRIPTING
using System;
using Unity.VisualScripting;


namespace StudioScor.MovementSystem.VisualScripting
{
    [UnitTitle("On Finished Movement")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\StudioScor\\MovementSystem")]
    public class MovementSystemOnFinishedMovementEventUnit : MovementSystemEventUnit
    {
        protected override string HookName => MovementSystemWithVisualScripting.MOVEMENTSYSTEM_ON_FINISHED_MOVEMENT;
    }
}
#endif