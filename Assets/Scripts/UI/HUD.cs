using System.Collections.Generic;
using Scripts.Game;
using Scripts.Items;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public Button[] ItemButtons;

    private GameSystem gameSystem;

    public void Initialize(GameSystem gameSystem)
    {
        this.gameSystem = gameSystem;

        for (int i = 0; i < ItemButtons.Length; i++)
        {
            var itemType = (ItemType) i;
            
            ItemButtons[i].onClick.AddListener(delegate
            {
                OnButtonClicked(itemType);
            });
        }
    }

    private void OnButtonClicked(ItemType itemType)
    {
        gameSystem.StartPlacingItem(itemType);
    }
}
