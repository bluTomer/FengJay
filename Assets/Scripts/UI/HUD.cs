using System;
using System.Collections;
using Scripts.Game;
using Scripts.Items;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField] private Button[] itemButtons;
    [SerializeField] private JayAnimator JayAnimator;

    private GameSystem gameSystem;
    private GraphicRaycaster raycaster;
    private Coroutine jayCoroutine;

    public void Initialize(GameSystem gameSystem)
    {
        this.gameSystem = gameSystem;
        raycaster = GetComponent<GraphicRaycaster>();

        for (int i = 0; i < itemButtons.Length; i++)
        {
            var itemType = (ItemType) i;
            
            itemButtons[i].onClick.AddListener(delegate
            {
                OnButtonClicked(itemType);
            });
        }
    }

    private void OnButtonClicked(ItemType itemType)
    {
        gameSystem.StartPlacingItem(itemType);
    }

    public void SetJayText(bool isHappy, float timePerText, Action uiDone, params string[] texts)
    {
        JayAnimator.SetHappy(isHappy);
        JayAnimator.Show();
        raycaster.enabled = false;

        if (jayCoroutine != null)
        {
            StopCoroutine(jayCoroutine);
        }
        
        jayCoroutine = StartCoroutine(RunJayText(timePerText, uiDone, texts));
    }
    
    private IEnumerator RunJayText(float timePerText, Action uiDone, string[] texts)
    {
        JayAnimator.SetText(texts[0]);

        bool finished;
        JayAnimator.UpdateTextEvent += () => finished = true;
        
        for (var index = 1; index < texts.Length; index++)
        {
            yield return new WaitForSeconds(timePerText);

            finished = false;
            
            JayAnimator.FlipTextBubble();

            yield return new WaitUntil(() => finished);
            
            JayAnimator.SetText(texts[index]);
        }
        
        yield return new WaitForSeconds(timePerText);
        
        JayAnimator.Hide();
        raycaster.enabled = true;
        jayCoroutine = null;

        if (uiDone != null)
            uiDone();
    }
}
