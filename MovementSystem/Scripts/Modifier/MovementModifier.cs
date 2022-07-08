using UnityEngine;


namespace KimScor.Movement
{
    public abstract class MovementModifier
    {
        public abstract Vector3 OnMovement(float deltaTime);
    }

}