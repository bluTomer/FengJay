using Scripts.Rules;
using UnityEngine;

namespace Scripts.System
{
    public class System
    {
        [SerializeField] private string configName;

        private Controller controller;
        private RuleChecker ruleChecker;
        private RuleSet ruleSet;
        
        private void Awake()
        {
            var config = Resources.Load<Config>("Config/" + configName);
            controller = Resources.Load<Controller>("Prefabs/Controller");
            ruleSet = Resources.Load<RuleSet>("Prefabs/RuleSet");
        }
    }
}