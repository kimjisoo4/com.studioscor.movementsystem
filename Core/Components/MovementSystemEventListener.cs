using UnityEngine;
using UnityEngine.Events;

using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public class MovementSystemEventListener : BaseStateMono
    {
        [Header(" [ On Jumped Listener  ")]
        [SerializeField] private EMovementSystemEventType _Type = EMovementSystemEventType.OnLanded;
        
        [Header(" [ Event ] ")]
        [SerializeField] private UnityEvent _OnInvokedEvent;
        public event UnityAction OnInvokeEvent;

        private IMovementSystem _MovementSystemEvent;

        private void Awake()
        {
            transform.TryGetComponentInParentOrChildren(out _MovementSystemEvent);
        }

        public override bool CanEnterState()
        {
            if (!base.CanEnterState())
                return false;

            if (_MovementSystemEvent is null)
                return false;

            return true;
        }
        protected override void EnterState()
        {
            switch (_Type)
            {
                case EMovementSystemEventType.OnJumped:
                    _MovementSystemEvent.OnJumped += MovementSystemEvent_Invoked;
                    break;
                case EMovementSystemEventType.OnLanded:
                    _MovementSystemEvent.OnLanded += MovementSystemEvent_Invoked;
                    break;
                case EMovementSystemEventType.OnStartedMovement:
                    _MovementSystemEvent.OnStartedMovement += MovementSystemEvent_Invoked;
                    break;
                case EMovementSystemEventType.OnFinishedMovement:
                    _MovementSystemEvent.OnFinishedMovement += MovementSystemEvent_Invoked;
                    break;
                default:
                    break;
            }
        }
        protected override void ExitState()
        {
            switch (_Type)
            {
                case EMovementSystemEventType.OnJumped:
                    _MovementSystemEvent.OnJumped -= MovementSystemEvent_Invoked;
                    break;
                case EMovementSystemEventType.OnLanded:
                    _MovementSystemEvent.OnLanded -= MovementSystemEvent_Invoked;
                    break;
                case EMovementSystemEventType.OnStartedMovement:
                    _MovementSystemEvent.OnStartedMovement -= MovementSystemEvent_Invoked;
                    break;
                case EMovementSystemEventType.OnFinishedMovement:
                    _MovementSystemEvent.OnFinishedMovement -= MovementSystemEvent_Invoked;
                    break;
                default:
                    break;
            }
        }

        private void MovementSystemEvent_Invoked(IMovementSystem movementSystem)
        {
            Log($" On Invoked Event [ {_Type} Event ]");

            _OnInvokedEvent?.Invoke();
            OnInvokeEvent?.Invoke();
        }

        
    }
}