﻿using System;
using System.Collections.Generic;
using Scripts.Game;
using Scripts.Items;
using Scripts.Rules;
using UnityEngine;

namespace Scripts
{
    public class Room : MonoBehaviour
    {
        public RoomPosition PositionPrefab;
        public Vector2Int Size;
        public Vector2 Resolution;
        public RoomPosition[,] Positions;

        private Transform positionParent;

        private void Awake()
        {
//            SetupNewRoom();
        }

        [ContextMenu("Setup")]
        public void SetupNewRoom(Level level)
        {
            // Increasing size by 1 to accomodate border tiles
            Size = level.LevelSize + (Vector2Int.one * 2);
            
            if (positionParent != null)
            {
                positionParent.DestroyGameObject();
            }

            positionParent = new GameObject("Positions").transform;
            positionParent.SetParent(transform);
            positionParent.localPosition = Vector3.zero;

            Positions = new RoomPosition[Size.x, Size.y];

            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    var position = Instantiate(PositionPrefab, positionParent);
                    position.Position = new Vector2Int(x, y);

                    position.transform.position = new Vector3(x * Resolution.x, transform.position.y, y * Resolution.y);
                    Positions[x, y] = position;
                }
            }

            // Add level objects (doors)
            foreach (var levelObject in level.Objects)
            {
                var item = Instantiate(GameSystem.Config.ItemSet.GetItemPrefab(levelObject.ItemType));
                var position = Positions[levelObject.Position.x + 1, levelObject.Position.y + 1];
                position.SetItem(item);
                item.SetPosition(position);

                for (int i = 0; i < (int)levelObject.ItemOrientation; i++)
                {
                    item.RotateRight();
                }
            }

            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    if (Positions[x, y].IsTaken)
                        continue;
                    
                    if (x == 0 || x == Size.x - 1 || y == 0 || y == Size.y - 1)
                    {
                        var item = Instantiate(GameSystem.Config.ItemSet.GetItemPrefab(ItemType.Blocked));
                        var position = Positions[x, y];
                        position.SetItem(item);
                        item.SetPosition(position);
                    }
                }
            }
        }

        public Item GetItemInRoom(ItemType type)
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    var position = Positions[x, y];

                    if (!position.IsTaken)
                        continue; // Empty position
                    
                    var item = position.Item;

                    if (item.Type == type)
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public List<Item> GetItemsInRoom(ItemType type)
        {
            var result = new List<Item>();
            
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    var position = Positions[x, y];

                    if (!position.IsTaken)
                        continue; // Empty position
                    
                    var item = position.Item;

                    if (item.Type == type)
                    {
                        result.Add(item);
                    }
                }
            }

            return result;
        }
        
        public void TraversePositions(Action<RoomPosition> invokeOnPosition)
        {
            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    var roomPos = GetPositionAt(x, y);
                    invokeOnPosition(roomPos);
                }
            }
        }

        public RoomPosition GetPositionAt(int x, int y)
        {
            if (x < 0 || x >= Size.x)
            {
                return null;
            }

            if (y < 0 || y >= Size.y)
            {
                return null;
            }

            return Positions[x, y];
        }

        public bool CanPlaceAtPosition(int x, int y)
        {
            if (x < 0 || x >= Size.x)
            {
                return false;
            }

            if (y < 0 || y >= Size.y)
            {
                return false;
            }

            var position = Positions[x, y];
            
            return !position.IsTaken;
        }
    }
}