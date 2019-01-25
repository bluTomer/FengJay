using System;
using UnityEngine;

namespace Scripts
{
    public class Item : MonoBehaviour
    {
        public enum PlacingType
        {
            None,
            NotAvailable,
            Available
        }
        
        [Header("Properties")]
        public Vector2Int Size;
        public Orientation Orientation;
        
        [SerializeField] private Renderer renderer;
        [SerializeField] private Transform pivot;
        
        public Color UnavailablePlacingColor;
        public Color AvailablePlacingColor;

        private Color originalColor;
        private bool showing = true;

        private void Awake()
        {
            originalColor = renderer.material.color;
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
                    renderer.material.color = UnavailablePlacingColor;
                    break;
                case PlacingType.Available:
                    renderer.material.color = AvailablePlacingColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("type", type, null);
            }
        }
    }
}