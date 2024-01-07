#if SCOR_ENABLE_VISUALSCRIPTING
using Unity.VisualScripting;
using UnityEngine;
using System;
using System.Collections;
using StudioScor.Utilities.VisualScripting;
using StudioScor.Utilities;


namespace StudioScor.MovementSystem.VisualScripting
{
    [UnitTitle("MovementSystem MoveToCurve")]
    [UnitShortTitle("MoveToCurve")]
    [UnitSubtitle("MovementSystem Unit")]
    [UnitCategory("StudioScor\\MovementSystem")]
    public class MoveToUnit : Unit
    {
        [DoNotSerialize]
        [NullMeansSelf]
        public ValueInput Target;

        [DoNotSerialize]
        public ControlInput Enter;
        [DoNotSerialize]
        public ControlOutput Exit;

        [DoNotSerialize]
        public ControlInput EnterFinished;
        [DoNotSerialize]
        public ControlOutput ExitFinished;

        [DoNotSerialize]
        public ControlInput EnterUpdate;
        [DoNotSerialize]
        public ControlOutput ExitUpdate;

        [DoNotSerialize]
        public ValueInput Direction;
        [DoNotSerialize]
        public ValueInput Distance;
        [DoNotSerialize]
        public ValueInput MoveCurve;

        [DoNotSerialize]
        public ValueInput Duration;
        [DoNotSerialize]
        public ValueInput EnterProgress;
        [DoNotSerialize]
        public ValueOutput ExitProgress;

        [Serialize]
        [Inspectable]
        [UnitHeaderInspectable("Main Progress Unit")]
        public bool IsMainProgress = true;

        private IMovementSystem movementSystem;

        private bool isPlaying = false;
        private Vector3 direction;
        private float distance;
        private float duration;
        private float elapsedTime;
        private float progress;
        private float prevValue;
        private AnimationCurve moveCurve;

        protected override void Definition()
        {
            Enter = ControlInput(nameof(Enter), OnEnter);
            Exit = ControlOutput(nameof(Exit));

            EnterUpdate = ControlInput(nameof(EnterUpdate), OnUpdate);
            ExitUpdate = ControlOutput(nameof(ExitUpdate));
            
            ExitFinished = ControlOutput(nameof(ExitFinished));

            Target = ValueInput<GameObject>(nameof(Target), null).NullMeansSelf();

            Direction = ValueInput<Vector3>(nameof(Direction), Vector3.forward);
            Distance = ValueInput<float>(nameof(Distance), 5f);
            MoveCurve = ValueInput<AnimationCurve>(nameof(MoveCurve), AnimationCurve.EaseInOut(0f,0f,1f,1f));

            if(IsMainProgress)
            {
                Duration = ValueInput<float>(nameof(Duration), 1f);
                ExitProgress = ValueOutput<float>(nameof(ExitProgress));
            }
            else
            {
                EnterFinished = ControlInput(nameof(EnterFinished), OnFinished);
                EnterProgress = ValueInput<float>(nameof(EnterProgress));
            }
        }

        private ControlOutput OnEnter(Flow flow)
        {
            if (isPlaying)
                return null;

            var actor = flow.GetValue<GameObject>(Target);

            if (actor && actor.TryGetMovementSystem(out movementSystem))
            {
                direction = flow.GetValue<Vector3>(Direction);
                distance = flow.GetValue<float>(Distance);
                moveCurve = flow.GetValue<AnimationCurve>(MoveCurve);

                if(IsMainProgress)
                {
                    duration = flow.GetValue<float>(Duration);
                }

                elapsedTime = 0f;
                progress = 0f;
                prevValue = 0f;

                isPlaying = true;
            }


            return Exit;
        }
        private ControlOutput OnUpdate(Flow flow)
        {
            if (!isPlaying)
                return null;

            if(IsMainProgress)
            {
                float deltaTime = Time.deltaTime;

                elapsedTime += deltaTime;

                progress = Mathf.Clamp01(elapsedTime.SafeDivide(duration));

                flow.SetValue(ExitProgress, progress);
            }
            else
            {
                progress = flow.GetValue<float>(EnterProgress);

            }

            var currentValue = moveCurve.Evaluate(progress) * distance;

            movementSystem.MovePosition(direction * (currentValue - prevValue));

            prevValue = currentValue;

            if (IsMainProgress && progress >= 1f)
            {
                isPlaying = false;

                flow.Invoke(ExitUpdate);

                flow.Invoke(ExitFinished);

                return null;
            }

            return ExitUpdate;
        }
        private ControlOutput OnFinished(Flow flow)
        {
            if (!isPlaying)
                return null;

            isPlaying = false;

            return ExitFinished;
        }
    }

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

            data.MovementSystem.MovePosition(position - data.PrevPosition);

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