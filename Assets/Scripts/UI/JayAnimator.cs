using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JayAnimator : MonoBehaviour
{
    public event Action UpdateTextEvent;

    private static readonly int showTrigger = Animator.StringToHash("Show");
    private static readonly int hideTrigger = Animator.StringToHash("Hide");
    private static readonly int happyBool = Animator.StringToHash("Happy");
    private static readonly int flipBubbleTrigger = Animator.StringToHash("FlipBubble");

    [SerializeField] private Text speechBubble;

    private Animator animator;
    private Coroutine wait;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Show()
    {
        animator.SetTrigger(showTrigger);
    }

    public void Hide()
    {
        animator.SetTrigger(hideTrigger);
    }

    public void SetHappy(bool happy)
    {
        animator.SetBool(happyBool, happy);
    }

    public void SwitchText(float delay)
    {
        if (wait != null)
        {
            StopCoroutine(wait);
        }

        wait = StartCoroutine(SwitchTextAfterDelay(delay));
    }

    public void InvokeUpdateTextEvent()
    {
        if (UpdateTextEvent != null)
            UpdateTextEvent();
    }

    public void SetText(string value)
    {
        speechBubble.text = value;
    }

    public void FlipTextBubble()
    {
        animator.SetTrigger(flipBubbleTrigger);
    }

    private IEnumerator SwitchTextAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        animator.SetTrigger(flipBubbleTrigger);

        wait = null;
    }
}
