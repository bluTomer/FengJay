using System;
using System.Collections.Generic;
using Scripts.Game;
using Scripts.Items;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scripts.Game
{
    [CreateAssetMenu(menuName = "Level Set")]
    public class LevelSet : ScriptableObject
    {
        public List<Level> Levels;
    }
}