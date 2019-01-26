using System;
using System.Collections;
using Scripts;
using Scripts.Game;
using Scripts.Items;
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
    }

    private void OnNewLevelStartedEvent(int levelIndex)
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
        jayAnimator.SetHappy(isHappy);
        jayAnimator.Show();
        raycaster.enabled = false;

        if (jayCoroutine != null)
        {
            StopCoroutine(jayCoroutine);
        }
        
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
