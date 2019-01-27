using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Items;
using Scripts.Rules;
using UnityEngine;

namespace Scripts.Game
{
    public class GameSystem : MonoBehaviour
    {
        public event Action<Level> NewLevelStartedEvent;
        public event Action<ItemType> NewItemPlacedEvent;
        public event Action<BaseRule> LevelFailedEvent;
        public event Action LevelCompletedEvent;
        
        public static Config Config { get; private set; }
        
        [SerializeField] private string configName;
        [SerializeField] private int StartingLevel;

        private Controller controller;
        private RuleChecker ruleChecker;
        private LevelSet levelSet;
        private ItemSet itemSet;
        private Camera camera;
        private Room room;
        private HUD hud;

        private List<Item> placedItems;
        
        public int CurrentLevelIndex { get; private set; }
        public List<ItemType> RequiredItemsForLevel { get; private set; }

        private void Awake()
        {
            if (Config != null)
                throw new Exception("More than 1 GameSystem object!");
            
            Config = Resources.Load<Config>("Config/" + configName);
            camera = FindObjectOfType<Camera>();
            levelSet = Config.levelSet;
            itemSet = Config.ItemSet;

            controller = Instantiate(Config.ControllerPrefab);
            room = Instantiate(Config.RoomPrefab);
            hud = Instantiate(Config.HUDPrefab);
            
            ruleChecker = new RuleChecker(room);
            placedItems = new List<Item>();

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
            CurrentLevelIndex = StartingLevel;
            StartLevel(CurrentLevelIndex);
        }

        public void StartLevel(int levelIndex)
        {
            var level = levelSet.Levels.ElementAt(levelIndex);
            RequiredItemsForLevel = new List<ItemType>(level.RequiredItems);
            
            foreach (var placedItem in placedItems)
            {
                placedItem.DestroyGameObject();
            }
            placedItems.Clear();
            
            LoadLevel(level);
            
            if (NewLevelStartedEvent != null)
                NewLevelStartedEvent(level);
        }

        public void LoadLevel(Level level)
        {
            room.SetupNewRoom(level);
            
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
            placedItems.Add(item);
            
            BaseRule failedRule;
            if (!ruleChecker.EvaluateRules(out failedRule))
            {
                if (LevelFailedEvent != null)
                    LevelFailedEvent(failedRule);
                
                SoundPlayer.Instance.PlaySound(Config.FailedRuleSound);
                Debug.Log("Rule Failed! " + failedRule.name);
                StartCoroutine(StartLevel());
            }
            else
            {
                RequiredItemsForLevel.Remove(item.Type);
                
                if (NewItemPlacedEvent != null)
                    NewItemPlacedEvent(item.Type);

                if (RequiredItemsForLevel.Count > 0)
                {
                    SoundPlayer.Instance.PlaySound(Config.PlaceSound);
                    return;
                }
                
                if (LevelCompletedEvent != null)
                    LevelCompletedEvent();

                SoundPlayer.Instance.PlaySound(Config.LevelSuccessSound);
                CurrentLevelIndex++;

                if (CurrentLevelIndex == levelSet.Levels.Count)
                {
                    // TODO: Win Game
                    return;
                }

                StartCoroutine(StartLevel());
            }
        }

        private IEnumerator StartLevel()
        {
            yield return new WaitForSeconds(1);
            StartLevel(CurrentLevelIndex);
        }
    }
}