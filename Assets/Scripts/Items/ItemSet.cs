using System;
using System.Collections.Generic;
using UnityEngine;

namespace Scripts.Items
{
    [CreateAssetMenu(menuName = "Item Set")]
    public class ItemSet : ScriptableObject
    {
        public List<ItemDefinition> Definitions;

        private Dictionary<ItemType, Item> Map;

        public Item GetItemPrefab(ItemType itemType)
        {
            if (Map == null)
            {
                InitMap();
            }

            Item item;
            return Map.TryGetValue(itemType, out item) ? item : null;
        }

        private void InitMap()
        {
            Map = new Dictionary<ItemType, Item>();
            
            foreach (var itemDefinition in Definitions)
            {
                Map.Add(itemDefinition.Type, itemDefinition.Prefab);
            }
        }
    }

    [Serializable]
    public class ItemDefinition
    {
        public ItemType Type;
        public Item Prefab;
    }
}