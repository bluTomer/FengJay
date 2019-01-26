using Scripts.Items;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rules/In Front Of Rule")]
    public class ShouldBeInFrontOfRule : BaseRule
    {
        private ItemType TestedItem;
        private ItemType InFrontOf;
        
        public override bool Evaluate()
        {
            var testedItem = Room.GetItemInRoom(TestedItem);

            if (testedItem == null)
                return true; 

            var inFrontOfItem = Room.GetItemInRoom(InFrontOf);
            if (inFrontOfItem == null)
                return true;

            var positionsInFront = testedItem.GetPositionsInFront(Room);

            foreach (var position in positionsInFront)
            {
                if (position.IsTaken && position.Item == inFrontOfItem)
                {
                    return true;
                }
            }

            return false;
        }
    }
}