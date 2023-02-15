namespace StudioScor.MovementSystem
{

    public interface IMovementModifier
    {
        public EMovementUpdateType UpdateType { get; }
        public void ProcessMovement(float deltaTime);
        public void ResetModifier();
    }

}