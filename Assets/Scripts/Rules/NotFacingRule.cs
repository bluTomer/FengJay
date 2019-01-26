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
            var item = Room.GetItemInRoom(ItemType);

            // If item isn't in the room the rule doesn't apply
            if (item == null)
                return true; 
            
            return item.Orientation != WrongOrientation;
        }
    }
}