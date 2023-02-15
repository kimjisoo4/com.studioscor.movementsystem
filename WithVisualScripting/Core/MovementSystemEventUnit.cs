#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using System;
using StudioScor.Utilities.VisualScripting;

namespace StudioScor.MovementSystem.VisualScripting
{
    public abstract class MovementSystemUnit : Unit
    {
        [DoNotSerialize]
        [PortLabel("MovementSystem")]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput MovementSystem { get; private set; }

        protected override void Definition()
        {
            MovementSystem = ValueInput<MovementSystemComponent>(nameof(MovementSystem), null).NullMeansSelf();
        }
    }

    public abstract class MovementSystemEventUnit : CustomEventUnit<MovementSystemComponent, EmptyEventArgs>
    {
        public override Type MessageListenerType => typeof(MovementSystemEventListener);
    }
}
#endif