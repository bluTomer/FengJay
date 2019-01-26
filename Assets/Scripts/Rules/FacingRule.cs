using Scripts.Items;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rules/Facing Rule")]
    public class FacingRule : BaseRule
    {
        public ItemType ItemType;
        public Orientation CorrectOrientation;

        public override bool Evaluate()
        {
            var item = Room.GetItemInRoom(ItemType);
            return item.Orientation == CorrectOrientation;
        }
    }
}