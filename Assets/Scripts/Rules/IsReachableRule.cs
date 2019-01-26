using System.Collections.Generic;
using Scripts.Items;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rule/Is All Reachable")]
    public class IsReachableRule : BaseRule
    {
        [SerializeField] private ItemType targetItemType;
        
        public override bool Evaluate()
        {
            var targetItems = Room.GetItemsInRoom(targetItemType);

            if (targetItems.Count == 0)
                return true;

            foreach (var targetItem in targetItems)
            {
                var reachableItems = targetItem.GetAllReachableItems(Room);

                var requiredItems = new List<Item>(targetItems);
                requiredItems.Remove(targetItem);
                
                foreach (var requiredItem in requiredItems)
                {
                    if (!reachableItems.Contains(requiredItem))
                        return false;
                }
            }

            return true;
        }

        public override string GetRuleAddedDescription()
        {
            return string.Format("The path between all {0}s must be clear.", targetItemType);
        }

        public override string GetRuleRemovedDescription()
        {
            throw new System.NotImplementedException();
        }
    }
}