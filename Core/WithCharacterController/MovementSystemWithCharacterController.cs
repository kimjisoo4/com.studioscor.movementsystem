using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{

    [AddComponentMenu("StudioScor/MovementSystem/Movement System With CharacterController", order:0)]
    public class MovementSystemWithCharacterController : MovementSystemComponent
    {
        [Header(" [ Use Character Controller ] ")]
        [SerializeField] private CharacterController _CharacterController;

        private Vector3 _LastVelocity;
        protected override Vector3 LastVelocity => _LastVelocity;

        private Vector3 _TeleporPositiont;
        private bool _WasTeleport;

        private void Reset()
        {
            gameObject.TryGetComponentInParentOrChildren(out _CharacterController);
        }

        protected override void OnSetup()
        {
            base.OnSetup();

            if (!_CharacterController)
            {
                if (!gameObject.TryGetComponentInParentOrChildren(out _CharacterController))
                {
                    Log("Character Contollre Is NULL", true);
                }
            }
        }

        protected override void OnMovement(float deltaTime)
        {
            _LastVelocity = _AddVelocity;

            if(_AddPosition != default)
            {
                _LastVelocity += _AddPosition / deltaTime;
            }

            _CharacterController.Move(_LastVelocity * deltaTime);

            if (_WasTeleport)
            {
                _WasTeleport = false;

                OnTeleport();
            }
        }

        private void OnTeleport()
        {
            _CharacterController.enabled = false;
            _CharacterController.transform.position = _TeleporPositiont;
            _CharacterController.enabled = true;
        }

        public override void Teleport(Vector3 position)
        {
            _TeleporPositiont = position;
            _WasTeleport = true;
        }
    }

}