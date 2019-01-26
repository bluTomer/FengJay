using Scripts.Items;
using Scripts.Rules;
using UnityEngine;

namespace Scripts.Game
{
    [CreateAssetMenu(menuName = "Config")]
    public class Config : ScriptableObject
    {
        public ItemSet ItemSet;
        public RuleSet RuleSet;

        public Controller ControllerPrefab;
        public Room RoomPrefab;
        public HUD HUDPrefab;
        
        public Color UnavailablePlacingColor;
        public Color AvailablePlacingColor;
    }
}