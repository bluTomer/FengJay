using UnityEngine;

namespace Scripts.Rules
{
    public abstract class BaseRule : ScriptableObject
    {
        protected Room Room;

        public virtual void Initialize(Room room)
        {
            Room = room;
        }

        public abstract bool Evaluate();
        public abstract string GetDescription();
    }
}