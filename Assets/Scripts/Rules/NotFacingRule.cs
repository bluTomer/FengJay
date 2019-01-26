using Scripts.Items;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rules/Not Facing Orientation")]
    public class NotFacingRule : BaseRule
    {
        public ItemType ItemType;
        public Orientation WrongOrientation;

        public override bool Evaluate()
        {
            var items = Room.GetItemsInRoom(ItemType);

            // If item isn't in the room the rule doesn't apply
            if (items.Count == 0)
                return true;

            foreach (var item in items)
            {
                if (item.Orientation == WrongOrientation)
                    return false;
            }

            return true;
        }
    }
}