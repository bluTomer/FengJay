using System;
using System.Collections.Generic;
using Scripts.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Rules
{
    [CreateAssetMenu(menuName = "Rule Set")]
    public class RuleSet : ScriptableObject
    {
        public List<Level> Levels;
    }

    [Serializable]
    public class Level
    {
        public Vector2Int LevelSize; // WithoutBorder
        public RoomPosition PositionPrefab;
        public List<ItemType> RequiredItems;
        
        [FormerlySerializedAs("Objects")]
        public List<LevelObjectDefinition> PrePlacedObjects;
        public List<BaseRule> RulesToAdd;
        public List<BaseRule> RulesToRemove;
    }

    [Serializable]
    public class LevelObjectDefinition
    {
        public Vector2Int Position;
        public Orientation ItemOrientation;
        public ItemType ItemType;
    }
}