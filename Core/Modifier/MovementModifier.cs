using UnityEngine;


namespace KimScor.MovementSystem
{
    public abstract class MovementModifier
    {
        public abstract Vector3 OnMovement(float deltaTime);
    }

}