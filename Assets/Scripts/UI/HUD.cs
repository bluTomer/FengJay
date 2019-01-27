using System;
using System.Collections;
using System.Collections.Generic;
using Scripts;
using Scripts.Game;
using Scripts.Items;
using Scripts.Rules;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public event Action RestartGameEvent;
    public event Action StartGameEvent;
    
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private JayAnimator jayAnimator;
    [SerializeField] private Button plantButtonPrefab;
    [SerializeField] private Button SofaButtonPrefab;
    [SerializeField] private GameObject GoodPlacementTag;
    [SerializeField] private GameObject WinScreen;

    private GameSystem gameSystem;
    private GraphicRaycaster raycaster;
    private Coroutine jayCoroutine;

    public void Initialize(GameSystem gameSystem)
    {
        this.gameSystem = gameSystem;
        raycaster = GetComponent<GraphicRaycaster>();

        gameSystem.NewLevelStartedEvent += OnNewLevelStartedEvent;
        gameSystem.NewItemPlacedEvent += OnNewItemPlacedEvent;
    }

    private void OnDestroy()
    {
        gameSystem.NewLevelStartedEvent -= OnNewLevelStartedEvent;
        gameSystem.NewItemPlacedEvent -= OnNewItemPlacedEvent;
    }

    public void OnWinGame()
    {
        WinScreen.SetActive(true);
    }

    public void OnStartGameButtonClicked()
    {
        if (StartGameEvent != null)
            StartGameEvent();
    }
    
    private void OnNewLevelStartedEvent(Level level)
    {
        RecreateNeededButtons();

        var texts = new List<string>();
        foreach (var rule in level.RulesToAdd)
        {
            texts.Add(rule.GetRuleAddedDescription());
        }

        SetJayText(true, 4, null, texts.ToArray());
    }

    private void OnNewItemPlacedEvent(ItemType itemType)
    {
        RecreateNeededButtons();
    }

    public void OnQuitButtonPressed()
    {
        Application.Quit();
    }

    public void OnLevelFailedEvent(BaseRule failedRule, Action callback)
    {
        SetJayText(false, 4, callback, failedRule.GetRuleAddedDescription());
    }

    private void OnLevelCompletedEvent()
    {
        RecreateNeededButtons();
    }

    private void RecreateNeededButtons()
    {
        var buttons = buttonsParent.GetComponentsInChildren<Button>();
        if (buttons != null)
        {
            foreach (var button in buttons)
            {
                button.DestroyGameObject();
            }
        }
        
        foreach (var requiredItem in gameSystem.RequiredItemsForLevel)
        {
            CreateButton(requiredItem);
        }
    }

    private void CreateButton(ItemType itemType)
    {
        Button prefab;
        switch (itemType)
        {
            case ItemType.Plant:
                prefab = plantButtonPrefab;
                break;
            case ItemType.Sofa:
                prefab = SofaButtonPrefab;
                break;
            default:
                throw new ArgumentOutOfRangeException("itemType", itemType, null);
        }
        
        var button = Instantiate(prefab, buttonsParent);
        button.onClick.AddListener(delegate
        {
            OnButtonClicked(itemType);
        });
    }

    private void OnButtonClicked(ItemType itemType)
    {
        gameSystem.StartPlacingItem(itemType);
    }

    public void SetJayText(bool isHappy, float timePerText, Action uiDone, params string[] texts)
    {
        if (texts == null || texts.Length == 0)
            return;
        
        if (jayCoroutine != null)
        {
            return;
        }

        if (!isHappy)
        {
            texts[0] += "\nTry Again...";
        }
        
        jayAnimator.SetHappy(isHappy);
        jayAnimator.Show();
        raycaster.enabled = false;

        
        jayCoroutine = StartCoroutine(RunJayText(timePerText, uiDone, texts));
    }

    private Coroutine GoodPlacement;
    
    public void ShowGoodPlacement()
    {
        if (GoodPlacement != null)
        {
            StopCoroutine(GoodPlacement);
        }
        
        GoodPlacementTag.SetActive(true);
        GoodPlacement = StartCoroutine(HideGoodPlacement());
    }

    private IEnumerator HideGoodPlacement()
    {
        yield return new WaitForSeconds(1);
        GoodPlacementTag.SetActive(false);

        GoodPlacement = null;
    }

    public void OnRestartButtonClicked()
    {
        if (RestartGameEvent != null)
            RestartGameEvent();
    }
    
    private IEnumerator RunJayText(float timePerText, Action uiDone, string[] texts)
    {
        jayAnimator.SetText(texts[0]);

        bool finished;
        jayAnimator.UpdateTextEvent += () => finished = true;
        
        for (var index = 1; index < texts.Length; index++)
        {
            yield return new WaitForSeconds(timePerText);

            finished = false;
            
            jayAnimator.FlipTextBubble();

            yield return new WaitUntil(() => finished);
            
            jayAnimator.SetText(texts[index]);
        }
        
        yield return new WaitForSeconds(timePerText);
        
        jayAnimator.Hide();
        raycaster.enabled = true;
        jayCoroutine = null;

        if (uiDone != null)
            uiDone();
    }
}
