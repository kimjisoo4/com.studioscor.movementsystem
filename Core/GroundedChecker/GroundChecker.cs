using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/GroundChecker", order: 0)]
    public abstract class GroundChecker : BaseMonoBehaviour
    {
        #region Event
        public delegate void ChangedGroundedStateHandler(GroundChecker groundChecker, bool isGrounded);
        #endregion
        [Header(" [ Ground Checker ] ")]
        [SerializeField] private LayerMask _GroundLayer;

        protected Vector3 _Normal;
        protected Vector3 _Point;
        protected float _Distance;
        protected bool _IsGrounded;
        protected bool _WasGrounded;

        protected LayerMask GroundLayer => _GroundLayer;
        public bool IsGrounded => _IsGrounded;
        public bool WasGrounded => _WasGrounded;
        public Vector3 Normal => _Normal;
        public Vector3 Point => _Point;
        public float Distance => _Distance;

        public event ChangedGroundedStateHandler OnChangedGroundedState;

        private void Awake()
        {
            OnSetup();
        }

        protected virtual void OnSetup() { }


        public void SetGrounded(bool isGrounded)
        {
            _WasGrounded = _IsGrounded;
            _IsGrounded = isGrounded;

            if (_IsGrounded == _WasGrounded)
                return;

            Callback_OnChangedGroundedState();
        }

        public abstract void CheckGrounded();

        protected void Callback_OnChangedGroundedState()
        {
            Log("On Changed Grounded State - " + _IsGrounded);

            OnChangedGroundedState?.Invoke(this, _IsGrounded);
        }
    }

}