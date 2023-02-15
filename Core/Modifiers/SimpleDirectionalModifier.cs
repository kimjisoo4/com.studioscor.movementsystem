using UnityEngine;
namespace StudioScor.MovementSystem
{
    [AddComponentMenu("StudioScor/MovementSystem/Modifiers/Simple Directional Modifier", order: 5)]
    public class SimpleDirectionalModifier : Modifier
    {
        [Header(" [ Simple Diretional Movement ] ")]
        [SerializeField] private float _MaxSpeed = 5f;

        public override void ProcessMovement(float deltaTime)
        {
            MovementSystem.AddVelocity(_MaxSpeed * MovementSystem.MoveDirection);
        }
    }

}