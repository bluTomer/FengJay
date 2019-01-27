using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Scripts.Items;
using Scripts.Rules;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Game
{
    public class GameSystem : MonoBehaviour
    {
        public event Action<Level> NewLevelStartedEvent;
        public event Action<ItemType> NewItemPlacedEvent;
        
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
        }

        private void AddListeners()
        {
            controller.ItemPlacedEvent += OnItemPlacedEvent;
            hud.RestartGameEvent += OnRestartGameEvent;
            hud.StartGameEvent += OnStartGameEvent;
        }
        
        private void RemoveListeners()
        {
            controller.ItemPlacedEvent -= OnItemPlacedEvent;
            hud.RestartGameEvent -= OnRestartGameEvent;
            hud.StartGameEvent -= OnStartGameEvent;
        }

        private void OnStartGameEvent()
        {
            StartGame();
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
                SoundPlayer.Instance.PlaySound(Config.FailedRuleSound);
                hud.OnLevelFailedEvent(failedRule, delegate
                {
                    Debug.Log("Rule Failed! " + failedRule.name);
                    StartLevel(CurrentLevelIndex);
                });
            }
            else
            {
                RequiredItemsForLevel.Remove(item.Type);
                
                if (NewItemPlacedEvent != null)
                    NewItemPlacedEvent(item.Type);

                if (RequiredItemsForLevel.Count > 0)
                {
                    SoundPlayer.Instance.PlaySound(Config.PlaceSound);
                    hud.ShowGoodPlacement();
                    return;
                }
                
                SoundPlayer.Instance.PlaySound(Config.LevelSuccessSound);

                if (CurrentLevelIndex == levelSet.Levels.Count)
                {
                    hud.OnWinGame();
                }
                else
                {
                    hud.SetJayText(true, 4, delegate
                    {
                        CurrentLevelIndex++;
                        StartLevel(CurrentLevelIndex);
                    }, "NICE VIBES!");
                }
            }
        }

        private void OnRestartGameEvent()
        {
            RemoveListeners();
            Config = null;
            SceneManager.LoadScene(0);
        }
    }
}