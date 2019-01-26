using System.Collections.Generic;
using Scripts.Items;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rule/Must Touch")]
    public class MustTouchRule : BaseRule
    {
        [SerializeField] private ItemType firstItemType;
        [SerializeField] private ItemType secondItemType;
        
        public override bool Evaluate()
        {
            var items = Room.GetItemsInRoom(firstItemType);

            bool allItemsResult = true;

            foreach (var item in items)
            {
                var surroundingPositions = item.GetSurroundingPositions(Room);

                bool result = false;
                
                foreach (var surroundingPosition in surroundingPositions)
                {
                    if (!surroundingPosition.IsTaken)
                        continue;

                    if (surroundingPosition.Item.Type != secondItemType)
                        continue;
                    
                    result = true;
                    break;
                }

                allItemsResult = allItemsResult && result;
            }

            return allItemsResult;
        }

        public override string GetRuleAddedDescription()
        {
            return string.Format("All {1}s must touch a {2}.", firstItemType, secondItemType);
        }
        
        public override string GetRuleRemovedDescription()
        {
            return string.Format("All {0}s no longer need to be touching a {1}.", firstItemType, secondItemType);
        }
    }
}