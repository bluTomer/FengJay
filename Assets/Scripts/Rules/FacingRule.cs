using Scripts.Items;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rule/Facing Orientation")]
    public class FacingRule : BaseRule
    {
        public ItemType ItemType;
        public Orientation CorrectOrientation;

        public override bool Evaluate()
        {
            var items = Room.GetItemsInRoom(ItemType);

            // If item isn't in the room the rule doesn't apply
            if (items.Count == 0)
                return true;

            foreach (var item in items)
            {
                if (item.Orientation != CorrectOrientation)
                    return false;
            }
            
            return true;
        }

        public override string GetRuleAddedDescription()
        {
            return string.Format("All {0}s must be facing {1}.", ItemType, CorrectOrientation);
        }

        public override string GetRuleRemovedDescription()
        {
            return string.Format("All {0}s no longer need to be facing {1}.", ItemType, CorrectOrientation);
        }
    }
}