using System.Collections;
using System.Collections.Generic;
using Scripts;
using Scripts.Items;
using Scripts.Rules;
using UnityEngine;

[CreateAssetMenu(menuName = "Rule/Facing Not Blocked")]
public class FacingNotBlockedRule : BaseRule
{
    [SerializeField] private ItemType TestedItem;
        
    public override bool Evaluate()
    {
        var testedItems = Room.GetItemsInRoom(TestedItem);

        if (testedItems.Count == 0)
            return true; 


        foreach (var testedItem in testedItems)
        {
            var positionsDirectlyInFront = testedItem.GetPositionsDirectlyInFront(Room);
            if (CheckPositionsForAnyItem(positionsDirectlyInFront))
                return false;
        }

        return true;
    }
        
    public override string GetRuleAddedDescription()
    {
        return string.Format("All {0}s must not be blocked.", TestedItem);
    }
        
    public override string GetRuleRemovedDescription()
    {
        return string.Format("All {0}s can now be blocked (but they don't have to).", TestedItem);
    }

    private bool CheckPositionsForAnyItem(List<RoomPosition> positionsInFront)
    {
        foreach (var position in positionsInFront)
        {
            if (position.IsTaken)
                return true;
        }

        return false;
    }
}
