using System;
using System.Collections.Generic;
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
        [SerializeField] private Orientation orientation;
        [SerializeField] private ItemType type;
        [SerializeField] private Bool5x5 size;

        private Renderer[] renderers;
        private Color[] originalColors;
        private bool showing = true;
        private bool[,] sizeTable;

        private void Awake()
        {
            renderers = GetComponentsInChildren<Renderer>(includeInactive: true);
            
            originalColors = new Color[renderers.Length];
            for (var index = 0; index < renderers.Length; index++)
            {
                originalColors[index] = renderers[index].material.color;
            }

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
                foreach (var renderer in renderers)
                {
                    renderer.enabled = true;
                }
                showing = true;
            }
        }

        public void Hide()
        {
            if (showing)
            {
                foreach (var renderer in renderers)
                {
                    renderer.enabled = false;
                }
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
                    for (var index = 0; index < renderers.Length; index++)
                    {
                        
                        renderers[index].material.color = originalColors[index];
                    }
                    break;
                case PlacingStatus.UnAvailable:
                    foreach (var renderer in renderers)
                    {
                        renderer.material.color = GameSystem.Config.UnavailablePlacingColor;
                    }
                    break;
                case PlacingStatus.Available:
                    foreach (var renderer in renderers)
                    {
                        renderer.material.color = GameSystem.Config.AvailablePlacingColor;
                    }
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
        
        public List<RoomPosition> GetPositionsDirectlyInFront(Room room)
        {
            var result = new List<RoomPosition>();
            
            var itemPositions = GetItemPositions(room);
            var advance = GetProgressDirection(orientation);

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
                    break;
                } 
            }

            return result;
        }
        
        public List<RoomPosition> GetPositionsInFront(Room room)
        {
            var result = new List<RoomPosition>();
            
            var itemPositions = GetItemPositions(room);
            var advance = GetProgressDirection(orientation);

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

        public List<RoomPosition> GetSurroundingPositions(Room room)
        {
            var result = new List<RoomPosition>();
            var itemPositions = GetItemPositions(room);

            foreach (var itemPosition in itemPositions)
            {
                // Go in all 4 directions until you get a position that is not the item
                for (int i = 0; i < 4; i++)
                {
                    var advance = GetProgressDirection((Orientation) i);
                    var pos = itemPosition;
                
                    while (pos != null)
                    {
                        pos = room.GetPositionAt(pos.Position.x + advance.x, pos.Position.y + advance.y);

                        if (pos == null)
                            continue;

                        if (result.Contains(pos))
                            break;
                        
                        if (pos.IsTaken && pos.Item == this)
                            continue;
                        
                        result.Add(pos);
                        break;
                    } 
                }
            }

            return result;
        }

        public List<Item> GetAllReachableItems(Room room)
        {
            var positions = GetItemPositions(room);

            var items = new List<Item>();

            foreach (var position in positions)
            {
                var reachableItems = GetAllReachableItems(position, new List<RoomPosition>(), room);
                items.AddRange(reachableItems);
            }

            return items;
        }

        private List<Item> GetAllReachableItems(RoomPosition position, List<RoomPosition> visitedPositions, Room room)
        {
            var itemList = new List<Item>();
            visitedPositions.Add(position);
            
            if (position.IsTaken && position.Item != this)
            {
                itemList.Add(position.Item);
                return itemList;
            }
            
            for (int i = 0; i < 4; i++)
            {
                var delta = GetProgressDirection((Orientation) i);
                var adjacentPosition = room.GetPositionAt(position.Position.x + delta.x, position.Position.y + delta.y);
                if (adjacentPosition != null && !visitedPositions.Contains(adjacentPosition))
                {
                    var reachableItems = GetAllReachableItems(adjacentPosition, visitedPositions, room);
                    itemList.AddRange(reachableItems);
                }
            }

            return itemList;
        }
        
        private Vector2Int GetProgressDirection(Orientation direction)
        {
            switch (direction)
            {
                case Orientation.Right:
                    return new Vector2Int(0, 1);
                case Orientation.Down:
                    return new Vector2Int(1, 0);
                case Orientation.Left:
                    return new Vector2Int(0, -1);
                case Orientation.Up:
                    return new Vector2Int(-1, 0);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}