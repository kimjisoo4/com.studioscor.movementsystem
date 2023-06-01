namespace StudioScor.MovementSystem
{

    public interface IMovementModifier
    {
        public EMovementUpdateType UpdateType { get; }
        public bool IsPlaying { get; }

        public void ProcessMovement(float deltaTime);
        public void ResetModifier();
        public void EnableModifier();
        public void DisableModifier();

    }

}