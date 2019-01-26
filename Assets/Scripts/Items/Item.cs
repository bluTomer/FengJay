using System;
using Boo.Lang;
using Scripts.Game;
using UnityEngine;

namespace Scripts.Items
{
    public class Item : MonoBehaviour
    {
        public const int MAX_SIZE = 5;
        
        public enum PlacingStatus
        {
            None,
            UnAvailable,
            Available
        }
        
        public Orientation Orientation
        {
            get { return orientation; }
        }

        public ItemType Type
        {
            get { return type; }
        }
        
        [SerializeField] private Transform pivot;
        [SerializeField] private Renderer renderer;
        [SerializeField] private Orientation orientation;
        [SerializeField] private Bool5x5 size;
        [SerializeField] private ItemType type;

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

        public void RotateRight()
        {
            var newOrientation = ((int) orientation + 1) % 4;
            orientation = (Orientation) newOrientation;

            pivot.Rotate(Vector3.up, 90.0f, Space.Self);
        }

        public void RotateLeft()
        {
            RotateRight();
            RotateRight();
            RotateRight();
        }

        public Vector2Int GetRotatedPoint(Vector2Int point)
        {
            Vector2Int rotatedPoint = point;
            int totalRotations = (int) orientation;
            
            for (int i = 0; i < totalRotations; i++)
            {
                var tempX = rotatedPoint.x;
                rotatedPoint.x = rotatedPoint.y;
                rotatedPoint.y = tempX;

                rotatedPoint.y = rotatedPoint.y * -1;
            }

            return rotatedPoint;
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

        public void SetPlacingStatus(PlacingStatus status)
        {
            switch (status)
            {
                case PlacingStatus.None:
                    renderer.material.color = originalColor;
                    break;
                case PlacingStatus.UnAvailable:
                    renderer.material.color = GameSystem.Config.UnavailablePlacingColor;
                    break;
                case PlacingStatus.Available:
                    renderer.material.color = GameSystem.Config.AvailablePlacingColor;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("status", status, null);
            }
        }

        public List<RoomPosition> GetItemPositions(Room room)
        {
            var result = new List<RoomPosition>();
            
            room.TraversePositions(delegate(RoomPosition position)
            {
                if (position == null)
                    return;

                if (!position.IsTaken)
                    return;

                if (position.Item == this)
                    result.Add(position);
            });

            return result;
        }
        
        public List<RoomPosition> GetPositionsInFront(Room room)
        {
            var result = new List<RoomPosition>();
            
            var itemPositions = GetItemPositions(room);
            var advance = GetProgressDirection();

            foreach (var itemPosition in itemPositions)
            {
                var pos = itemPosition;
                
                while (pos != null)
                {
                    pos = room.GetPositionAt(pos.Position.x + advance.x, pos.Position.y + advance.y);

                    if (pos == null)
                        continue;
                        
                    if (pos.IsTaken && pos.Item == this)
                        continue;

                    result.Add(pos);
                } 
            }

            return result;
        }

        private Vector2Int GetProgressDirection()
        {
            switch (orientation)
            {
                case Orientation.Forward:
                    return Vector2Int.right;
                case Orientation.Right:
                    return Vector2Int.down;
                case Orientation.Back:
                    return Vector2Int.left;
                case Orientation.Left:
                    return Vector2Int.up;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}