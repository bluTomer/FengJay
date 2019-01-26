using System;
using Scripts.Game;
using Scripts.Items;
using UnityEngine;

namespace Scripts
{
    public class Controller : MonoBehaviour
    {
        public enum ControlMode
        {
            None,
            PlacingObject,
        }

        public event Action<Item> ItemPlacingEvent;
        public event Action<Item> ItemPlacedEvent;
        
        public Camera camera;
        public LayerMask RaycastHitMask;
        public ControlMode CurrentMode;
        public Item ItemPrefab;
        
        private Room room;
        private Item itemBeingPlaced;

        public void Initialize(Room room, Camera camera)
        {
            this.room = room;
            this.camera = camera;
        }
        
        private void Update()
        {
            CheckModes();
            CheckRotate();
        }

        private void CheckModes()
        {
            if (CurrentMode == ControlMode.None && Input.GetKeyDown(KeyCode.P))
            {
                StartPlacing(CreateItem());
            }

            if (CurrentMode == ControlMode.PlacingObject && Input.GetKeyDown(KeyCode.Escape))
            {
                StopPlacing();
            }
            
            if (CurrentMode == ControlMode.PlacingObject)
            {
                TryPlacingObject();
            }
        }

        public void StartPlacing(Item item)
        {
            if (CurrentMode == ControlMode.PlacingObject)
            {
                StopPlacing();
            }
            
            itemBeingPlaced = item;
            CurrentMode = ControlMode.PlacingObject;
        }

        public void StopPlacing()
        {
            CurrentMode = ControlMode.None;
            itemBeingPlaced.DestroyGameObject();
            itemBeingPlaced = null;
        }

        private void CheckRotate()
        {
            if (CurrentMode == ControlMode.None)
                return;
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                itemBeingPlaced.RotateRight();
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                itemBeingPlaced.RotateLeft();
            }
        }

        private void TryPlacingObject()
        {
            var raycast = PointerRaycast();

            if (raycast == null)
            {
                itemBeingPlaced.Hide();
                return;
            }
            
            itemBeingPlaced.Show();
            
            var position = raycast.hit.rigidbody.GetComponent<RoomPosition>();
            if (position != null)
            {
                var placeSuccess = TryPlacingAtPosition(position);
                if (!placeSuccess)
                {
                    PlaceItemAtPoint(raycast.hit.point);
                }
            }
            
            var room = raycast.hit.rigidbody.GetComponent<Room>();
            if (room != null)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    SoundPlayer.Instance.PlaySound(GameSystem.Config.ErrorSound);
                }
                PlaceItemAtPoint(raycast.hit.point);
            }
        }

        private bool TryPlacingAtPosition(RoomPosition position)
        {
            for (int x = 0; x < Item.MAX_SIZE; x++)
            {
                for (int y = 0; y < Item.MAX_SIZE; y++)
                {
                    if (itemBeingPlaced.ExistsInPos(x, y))
                    {
                        var rotatedPos = itemBeingPlaced.GetRotatedPoint(new Vector2Int(x, y));
                        
                        if (!room.CanPlaceAtPosition(position.Position.x + rotatedPos.x, position.Position.y + rotatedPos.y))
                        {
                            // Position not available
                            itemBeingPlaced.SetPlacingStatus(Item.PlacingStatus.UnAvailable);
                            return false;
                        }
                    }
                }
            }
            
            // Object can be placed
            itemBeingPlaced.SetPosition(position);
            itemBeingPlaced.SetPlacingStatus(Item.PlacingStatus.Available);

            if (Input.GetMouseButtonDown(0))
            {
                if (ItemPlacingEvent != null)
                    ItemPlacingEvent(itemBeingPlaced);
                
                // Place item
                for (int x = 0; x < Item.MAX_SIZE; x++)
                {
                    for (int y = 0; y < Item.MAX_SIZE; y++)
                    {
                        if (itemBeingPlaced.ExistsInPos(x, y))
                        {
                            var rotatedPos = itemBeingPlaced.GetRotatedPoint(new Vector2Int(x, y));
                            var objectPosition = room.GetPositionAt(position.Position.x + rotatedPos.x, position.Position.y + rotatedPos.y);
                            objectPosition.SetItem(itemBeingPlaced);
                        }
                    }
                }
                
                itemBeingPlaced.SetPlacingStatus(Item.PlacingStatus.None);
                CurrentMode = ControlMode.None;
                
                SoundPlayer.Instance.PlaySound(GameSystem.Config.PlaceSound);
                
                if (ItemPlacedEvent != null)
                    ItemPlacedEvent(itemBeingPlaced);
            }

            return true;
        }

        private void PlaceItemAtPoint(Vector3 point)
        {
            itemBeingPlaced.transform.position = point;
            itemBeingPlaced.SetPlacingStatus(Item.PlacingStatus.UnAvailable);
        }
        
        private Hit PointerRaycast()
        {
            var ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, float.MaxValue, RaycastHitMask))
            {
                return new Hit(hit);
            }

            return null;
        }

        private Item CreateItem()
        {
            var item = Instantiate(ItemPrefab);
            item.Hide();
            return item;
        }
    }
}