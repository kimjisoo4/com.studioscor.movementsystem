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
        protected Vector3 _normal;
        protected Vector3 _point;
        protected float _distance;
        protected bool _isGrounded;
        protected bool _wasGrounded;


        public bool IsGrounded => _isGrounded;
        public bool WasGrounded => _wasGrounded;
        public Vector3 Normal => _normal;
        public Vector3 Point => _point;
        public float Distance => _distance;

        public event IGroundChecker.ChangedGroundedStateHandler OnChangedGroundedState;

        private void Awake()
        {
            OnSetup();
        }

        protected virtual void OnSetup() { }


        public void SetGrounded(bool isGrounded)
        {
            _wasGrounded = _isGrounded;
            _isGrounded = isGrounded;

            if (_isGrounded == _wasGrounded)
                return;

            Callback_OnChangedGroundedState();
        }

        public abstract void CheckGrounded();

        protected void Callback_OnChangedGroundedState()
        {
            Log("On Changed Grounded State - " + _isGrounded);

            OnChangedGroundedState?.Invoke(this, _isGrounded);
        }
    }

}