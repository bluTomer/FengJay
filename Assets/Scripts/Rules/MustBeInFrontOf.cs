using Scripts.Items;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rules/Must Be In Front Of")]
    public class MustBeInFrontOfRule : BaseRule
    {
        [SerializeField] private ItemType TestedItem;
        [SerializeField] private ItemType InFrontOf;
        
        public override bool Evaluate()
        {
            var testedItem = Room.GetItemInRoom(TestedItem);

            if (testedItem == null)
                return true; 

            var inFrontOfItem = Room.GetItemInRoom(InFrontOf);
            if (inFrontOfItem == null)
                return false;

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