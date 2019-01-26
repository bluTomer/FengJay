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
    }
}