using System;
using System.Linq;
using Scripts.Items;
using Scripts.Rules;
using UnityEngine;

namespace Scripts.Game
{
    public class GameSystem : MonoBehaviour
    {
        public event Action<int> NewLevelStartingEvent;
        
        public static Config Config { get; private set; }
        
        [SerializeField] private string configName;

        private Controller controller;
        private RuleChecker ruleChecker;
        private RuleSet ruleSet;
        private ItemSet itemSet;
        private Camera camera;
        private Room room;
        private HUD hud;
        
        public int CurrentLevelIndex { get; private set; }

        private void Awake()
        {
            if (Config != null)
                throw new Exception("More than 1 GameSystem object!");
            
            Config = Resources.Load<Config>("Config/" + configName);
            camera = FindObjectOfType<Camera>();
            ruleSet = Config.RuleSet;
            itemSet = Config.ItemSet;

            controller = Instantiate(Config.ControllerPrefab);
            room = Instantiate(Config.RoomPrefab);
            hud = Instantiate(Config.HUDPrefab);
            
            ruleChecker = new RuleChecker(room);

            controller.Initialize(room, camera);
            hud.Initialize(this);

            AddListeners();
            StartGame();
        }

        private void AddListeners()
        {
            controller.ItemPlacedEvent += OnItemPlacedEvent;
        }

        public void StartGame()
        {
            CurrentLevelIndex = 0;
            StartLevel(CurrentLevelIndex);
        }

        public void StartLevel(int levelIndex)
        {
            if (NewLevelStartingEvent != null)
                NewLevelStartingEvent(levelIndex);
            
            var level = ruleSet.Levels.ElementAt(levelIndex);
            LoadLevel(level);
        }

        public void LoadLevel(Level level)
        {
            room.SetupNewRoom();
            
            foreach (var rule in level.RulesToAdd)
            {
                ruleChecker.AddRule(rule);
            }

            foreach (var rule in level.RulesToRemove)
            {
                ruleChecker.RemoveRule(rule);
            }
        }

        public void StartPlacingItem(ItemType itemType)
        {
            var prefab = itemSet.GetItemPrefab(itemType);
            var item = Instantiate(prefab);
            controller.StartPlacing(item);
        }

        private void OnItemPlacedEvent(Item item)
        {
            BaseRule failedRule;
            if (!ruleChecker.EvaluateRules(out failedRule))
            {
                // TODO: Notify failed rule
            }
            else
            {
                // TODO: Notify success
            }
        }
    }
}