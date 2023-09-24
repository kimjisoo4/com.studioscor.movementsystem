using UnityEngine;
using StudioScor.Utilities;

namespace StudioScor.MovementSystem
{
    public interface IGroundChecker
    {
        #region Event
        public delegate void ChangedGroundedStateHandler(GroundChecker groundChecker, bool isGrounded);
        #endregion

        public void SetGrounded(bool isGrounded);
        public void CheckGrounded();

        public bool IsGrounded { get; }
        public bool WasGrounded { get; }
        public Vector3 Normal { get; }
        public Vector3 Point { get; }
        public float Distance { get; }

        public event ChangedGroundedStateHandler OnChangedGroundedState;
    }

    [AddComponentMenu("StudioScor/MovementSystem/GroundChecker", order: 0)]
    public abstract class GroundChecker : BaseMonoBehaviour, IGroundChecker
    {
        [Header(" [ Ground Checker ] ")]
        protected Vector3 _Normal;
        protected Vector3 _Point;
        protected float _Distance;
        protected bool _IsGrounded;
        protected bool _WasGrounded;


        public bool IsGrounded => _IsGrounded;
        public bool WasGrounded => _WasGrounded;
        public Vector3 Normal => _Normal;
        public Vector3 Point => _Point;
        public float Distance => _Distance;

        public event IGroundChecker.ChangedGroundedStateHandler OnChangedGroundedState;

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