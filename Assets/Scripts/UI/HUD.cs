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
    [SerializeField] private Transform buttonsParent;
    [SerializeField] private JayAnimator jayAnimator;
    [SerializeField] private Button buttonPrefab;

    private GameSystem gameSystem;
    private GraphicRaycaster raycaster;
    private Coroutine jayCoroutine;

    public void Initialize(GameSystem gameSystem)
    {
        this.gameSystem = gameSystem;
        raycaster = GetComponent<GraphicRaycaster>();

        gameSystem.NewLevelStartedEvent += OnNewLevelStartedEvent;
        gameSystem.NewItemPlacedEvent += OnNewItemPlacedEvent;
        gameSystem.LevelFailedEvent += OnLevelFailedEvent;
        gameSystem.LevelCompletedEvent += OnLevelCompletedEvent;
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

    private void OnLevelFailedEvent(BaseRule failedRule)
    {
        SetJayText(false, 4, null, failedRule.GetRuleAddedDescription());
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
        var button = Instantiate(buttonPrefab, buttonsParent);
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
        if (jayCoroutine != null)
        {
            return;
            StopCoroutine(jayCoroutine);
        }
        
        jayAnimator.SetHappy(isHappy);
        jayAnimator.Show();
        raycaster.enabled = false;

        
        jayCoroutine = StartCoroutine(RunJayText(timePerText, uiDone, texts));
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
