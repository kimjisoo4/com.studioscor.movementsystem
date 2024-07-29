using UnityEngine;

namespace StudioScor.MovementSystem
{
    public static class MovementSystemUtility
    {
        #region Get MovementSystem
        public static IMovementSystem GetMovementSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<IMovementSystem>();
        }
        public static IMovementSystem GetMovementSystem(this Component component)
        {
            var movementSystem = component as IMovementSystem;

            if (movementSystem is not null)
                return movementSystem;

            return component.GetComponent<IMovementSystem>();
        }
        public static bool TryGetMovementSystem(this GameObject gameObject, out IMovementSystem movementSystem)
        {
            return gameObject.TryGetComponent(out movementSystem);
        }
        public static bool TryGetMovementSystem(this Component component, out IMovementSystem movementSystem)
        {
            movementSystem = component as IMovementSystem;

            if (movementSystem is not null)
                return true;

            return component.TryGetComponent(out movementSystem);
        }
        #endregion
        #region Get Movement Module System
        public static IMovementModuleSystem GetMovementModuleSystem(this GameObject gameObject)
        {
            return gameObject.GetComponent<IMovementModuleSystem>();
        }
        public static IMovementModuleSystem GetMovementModuleSystem(this Component component)
        {
            var movementSystem = component as IMovementModuleSystem;

            if (movementSystem is not null)
                return movementSystem;

            return component.GetComponent<IMovementModuleSystem>();
        }
        public static bool TryGetMovementModuleSystem(this GameObject gameObject, out IMovementModuleSystem movementModuleSystem)
        {
            return gameObject.TryGetComponent(out movementModuleSystem);
        }
        public static bool TryGetMovementModuleSystem(this Component component, out IMovementModuleSystem movementModuleSystem)
        {
            movementModuleSystem = component as IMovementModuleSystem;

            if (movementModuleSystem is not null)
                return true;

            return component.TryGetComponent(out movementModuleSystem);
        }
        #endregion
        #region Get Ground Checker

        public static IGroundChecker GetGroundChecker(this GameObject gameObject)
        {
            return gameObject.GetComponent<IGroundChecker>();
        }
        public static IGroundChecker GetGroundChecker(this Component component)
        {
            var groundChecker = component as IGroundChecker;

            if (groundChecker is not null)
                return groundChecker;

            return component.GetComponent<IGroundChecker>();
        }
        public static bool TryGetGroundChecker(this GameObject gameObject, out IGroundChecker groundChecker)
        {
            return gameObject.TryGetComponent(out groundChecker);
        }
        public static bool TryGetGroundChecker(this Component component, out IGroundChecker groundChecker)
        {
            groundChecker = component as IGroundChecker;

            if (groundChecker is not null)
                return true;

            return component.TryGetComponent(out groundChecker);
        }
        #endregion

        public static bool TryGetModifier<T>(this IMovementModuleSystem movementModuleSystem, out T movementModifier) where T : IMovementModifier
        {
            foreach (var modifier in movementModuleSystem.Modifiers)
            {
                if (modifier.GetType() == typeof(T))
                {
                    movementModifier = (T)modifier;

                    return true;
                }
            }
            
            movementModifier = default(T);

            return false;
        }
    }

}