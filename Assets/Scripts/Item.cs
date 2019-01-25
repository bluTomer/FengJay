using System;
using UnityEngine;

namespace Scripts
{
    public class Item : MonoBehaviour
    {
        public const int MAX_SIZE = 5;
        
        public enum PlacingType
        {
            None,
            NotAvailable,
            Available
        }
        
        [SerializeField] private Transform pivot;
        [SerializeField] private Renderer renderer;
        [SerializeField] private Color unavailablePlacingColor;
        [SerializeField] private Color availablePlacingColor;
        [SerializeField] private Orientation orientation;
        [SerializeField] private Bool5x5 size;

        private Color originalColor;
        private bool showing = true;
        private bool[,] sizeTable;

        private void Awake()
        {
            originalColor = renderer.material.color;
            sizeTable = new bool[MAX_SIZE, MAX_SIZE];
            for (int x = 0; x < MAX_SIZE; x++)
            {
                var currentRow = size.Rows[x];
                
                for (int y = 0; y < MAX_SIZE; y++)
                {
                    sizeTable[x, y] = currentRow.Row[y];
                }
            }
        }

        public bool ExistsInPos(int x, int y)
        {
            if (x < 0 || x >= MAX_SIZE)
            {
                return false;
            }

            if (y < 0 || y >= MAX_SIZE)
            {
                return false;
            }

            return sizeTable[x, y];
        }

        public void Show()
        {
            if (!showing)
            {
                renderer.enabled = true;
                showing = true;
            }
        }

        public void Hide()
        {
            if (showing)
            {
                renderer.enabled = false;
                showing = false;
            }
        }

        public void SetPosition(RoomPosition position)
        {
            transform.position = position.transform.position;
        }

        public void SetPlacingType(PlacingType type)
        {
            switch (type)
            {
                case PlacingType.None:
                    renderer.material.color = originalColor;
                    break;
                case PlacingType.NotAvailable:
                    renderer.material.color = unavailablePlacingColor;
                    break;
                case PlacingType.Available:
                    renderer.material.color = availablePlacingColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}