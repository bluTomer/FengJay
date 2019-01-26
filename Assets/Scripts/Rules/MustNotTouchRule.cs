using System.Collections.Generic;
using Scripts.Items;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rule/Must Not Touch")]
    public class MustNotTouchRule : BaseRule
    {
        [SerializeField] private List<ItemType> itemTypes;
        
        public override bool Evaluate()
        {
            foreach (var itemType in itemTypes)
            {
                var items = Room.GetItemsInRoom(itemType);

                foreach (var item in items)
                {
                    var surroundingPositions = item.GetSurroundingPositions(Room);

                    foreach (var surroundingPosition in surroundingPositions)
                    {
                        if (!surroundingPosition.IsTaken)
                            continue;

                        if (itemTypes.Contains(surroundingPosition.Item.Type))
                            return false;
                    }
                }
            }

            return true;
        }
    }
}