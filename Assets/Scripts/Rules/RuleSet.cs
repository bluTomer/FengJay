using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rules/Rule Set")]
    public class RuleSet : ScriptableObject
    {
        public List<Level> Levels;
    }

    [Serializable]
    public class Level
    {
        public List<BaseRule> RulesToAdd;
        public List<BaseRule> RulesToRemove;
    }
}