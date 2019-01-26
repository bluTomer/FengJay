using System.Collections.Generic;
using Scripts.Items;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rules/Should Be In Front Of")]
    public class ShouldBeInFrontOfRule : BaseRule
    {
        [SerializeField] private ItemType TestedItem;
        [SerializeField] private ItemType InFrontOf;
        
        public override bool Evaluate()
        {
            var testedItems = Room.GetItemsInRoom(TestedItem);

            if (testedItems.Count == 0)
                return true; 

            var inFrontOfItem = Room.GetItemInRoom(InFrontOf);
            if (inFrontOfItem == null)
                return true;

            bool result = true;
            
            foreach (var testedItem in testedItems)
            {
                var positionsInFront = testedItem.GetPositionsInFront(Room);
                result = result && CheckPositionsForItem(positionsInFront, inFrontOfItem);
            }

            return result;
        }

        private bool CheckPositionsForItem(List<RoomPosition> positionsInFront, Item inFrontOfItem)
        {
            foreach (var position in positionsInFront)
            {
                if (position.IsTaken && position.Item == inFrontOfItem)
                    return true;
            }

            return false;
        }
    }
}