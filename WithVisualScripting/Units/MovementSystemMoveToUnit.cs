#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections;
using StudioScor.Utilities.VisualScripting;

namespace StudioScor.MovementSystem.VisualScripting
{
    [UnitTitle("MovementSystem MoveTo")]
    [UnitShortTitle("MoveTo")]
    [UnitSubtitle("MovementSystem Wait")]
    [UnitCategory("Time\\StudioScor\\MovementSystem")]
    public class MovementSystemMoveToUnit : UpdateUnit
    {
        [DoNotSerialize]
        [PortLabel("MovementSystem")]
        [PortLabelHidden]
        [NullMeansSelf]
        public ValueInput MovementSystem { get; private set; }

        [DoNotSerialize]
        [PortLabel("Finished")]
        public ControlOutput Finished { get; private set; }

        [DoNotSerialize]
        [PortLabel("Position")]
        [PortLabelHidden]
        public ValueInput Position { get; private set; }

        [DoNotSerialize]
        [PortLabel("Duration")]
        [PortLabelHidden]
        public ValueInput Duration { get; private set; }

        [DoNotSerialize]
        [PortLabel("Curve")]
        [PortLabelHidden]
        public ValueInput Curve { get; private set; }

        [DoNotSerialize]
        [PortLabel("Elapsed(%)")]
        public ValueOutput NormalizedTime { get; private set; }

        protected override void Definition()
        {
            base.Definition();

            Finished = ControlOutput(nameof(Finished));
            MovementSystem = ValueInput<MovementSystemComponent>(nameof(MovementSystem), null).NullMeansSelf();

            Position = ValueInput<Vector3>(nameof(Position), default);

            if (!IsManualNormalizedTime)
            {
                Duration = ValueInput<float>(nameof(Duration), 0.2f);

                Requirement(Duration, Enter);
            }

            Curve = ValueInput<AnimationCurve>(nameof(Curve), AnimationCurve.EaseInOut(0f, 0f, 1f, 1f));

            NormalizedTime = ValueOutput<float>(nameof(NormalizedTime));

            Succession(Enter, Exit);
            Succession(Enter, Update);
            Succession(Enter, Finished);

            Assignment(Enter, NormalizedTime);

            Requirement(Position, Enter);
            Requirement(Curve, Enter);

            ManualDefinition();
        }


        public new class Data : UpdateUnit.Data
        {
            public MovementSystemComponent MovementSystem;
            public float Duration;
            public Vector3 Position;
            public AnimationCurve Curve;

            public float ElapsedTime;
            public float NormalizedTime;
            public Vector3 PrevPosition;
        }
        public override IGraphElementData CreateData()
        {
            return new Data();
        }


        protected override void OnUpdate(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            UpdateMovement(flow, data);
        }

        protected override ControlOutput OnManualUpdate(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            UpdateMovement(flow, data);

            return null;
        }

        protected override void OnEnter(Flow flow)
        {
            var data = flow.stack.GetElementData<Data>(this);

            data.MovementSystem = flow.GetValue<MovementSystemComponent>(MovementSystem);

            data.Duration = IsManualNormalizedTime ? 0f : flow.GetValue<float>(Duration);
            data.Position = flow.GetValue<Vector3>(Position);
            data.Curve = flow.GetValue<AnimationCurve>(Curve);

            data.IsActivate = true;
            data.ElapsedTime = 0f;
            data.NormalizedTime = 0f;
            data.PrevPosition = Vector3.zero;

            flow.SetValue(NormalizedTime, data.NormalizedTime);
        }

        protected override void OnInterrupt(Flow flow)
        {

        }

        public void UpdateMovement(Flow flow, Data data)
        {
            if(IsManualNormalizedTime)
            {
                var normaliedTime = flow.GetValue<float>(ManualNormalizedTime);

                data.NormalizedTime = Mathf.Max(normaliedTime, data.NormalizedTime);
            }
            else
            {
                if (IsManualDeltaTime)
                {
                    data.ElapsedTime += flow.GetValue<float>(ManualDeltaTime);
                }
                else
                {
                    data.ElapsedTime += DeltaTime;
                }

                data.ElapsedTime = Mathf.Min(data.ElapsedTime, data.Duration);

                data.NormalizedTime = data.ElapsedTime / data.Duration;
                data.NormalizedTime = MathF.Min(data.NormalizedTime, 1f);
            }

            Vector3 position = data.Position * data.Curve.Evaluate(data.NormalizedTime);

            data.MovementSystem.AddPosition(position - data.PrevPosition);

            data.PrevPosition = position;

            flow.SetValue(NormalizedTime, data.NormalizedTime);

            flow.Invoke(Update);

            if (data.NormalizedTime >= 1f)
            {
                data.IsActivate = false;

                flow.Invoke(Finished);
            }
        }
    }

}
#endif