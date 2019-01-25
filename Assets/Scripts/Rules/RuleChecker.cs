using System.Collections.Generic;

namespace Scripts.Rules
{
    public class RuleChecker
    {
        public readonly List<BaseRule> ActiveRules;

        private Room room;

        public RuleChecker(Room room)
        {
            this.room = room;
            ActiveRules = new List<BaseRule>();
        }
        
        public void AddRule(BaseRule rule)
        {
            rule.Initialize(room);
            ActiveRules.Add(rule);
        }

        public void RemoveRule(BaseRule rule)
        {
            ActiveRules.Remove(rule);
        }

        public bool EvaluateRules(out BaseRule failedRule)
        {
            foreach (var activeRule in ActiveRules)
            {
                var rulePassed = activeRule.Evaluate();
                if (!rulePassed)
                {
                    failedRule = activeRule;
                    return false;
                }
            }

            failedRule = null;
            return true;
        }
    }
}