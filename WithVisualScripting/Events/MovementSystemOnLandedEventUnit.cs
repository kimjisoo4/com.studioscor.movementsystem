#if SCOR_ENABLE_VISUALSCRIPTING
using System;
using Unity.VisualScripting;


namespace StudioScor.MovementSystem.VisualScripting
{
    [UnitTitle("On Landed")]
    [UnitSubtitle("Event")]
    [UnitCategory("Events\\StudioScor\\MovementSystem")]
    public class MovementSystemOnLandedEventUnit : MovementSystemEventUnit
    {
        protected override string HookName => MovementSystemWithVisualScripting.MOVEMENTSYSTEM_ON_LANDED;
    }
}
#endif