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
        
        public Camera Camera;
        public LayerMask RaycastHitMask;
        public ControlMode CurrentMode;
        public Item ItemPrefab;
        public Room Room;

        private Item itemBeingPlaced;
        
        private void Update()
        {
            CheckModes();
            CheckRotate();
        }

        private void CheckModes()
        {
            if (CurrentMode == ControlMode.None && Input.GetKeyDown(KeyCode.P))
            {
                itemBeingPlaced = CreateItem();
                CurrentMode = ControlMode.PlacingObject;
            }

            if (CurrentMode == ControlMode.PlacingObject && Input.GetKeyDown(KeyCode.Escape))
            {
                CurrentMode = ControlMode.None;
                itemBeingPlaced.DestroyGameObject();
                itemBeingPlaced = null;
            }
            
            if (CurrentMode == ControlMode.PlacingObject)
            {
                TryPlacingObject();
            }
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
                        
                        if (!Room.CanPlaceAtPosition(position.Position.x + rotatedPos.x, position.Position.y + rotatedPos.y))
                        {
                            // Position not available
                            itemBeingPlaced.SetPlacingType(Item.PlacingType.NotAvailable);
                            return false;
                        }
                    }
                }
            }
            
            // Object can be placed
            itemBeingPlaced.SetPosition(position);
            
            itemBeingPlaced.SetPlacingType(Item.PlacingType.Available);

            if (Input.GetMouseButtonDown(0))
            {
                // Place item
                for (int x = 0; x < Item.MAX_SIZE; x++)
                {
                    for (int y = 0; y < Item.MAX_SIZE; y++)
                    {
                        if (itemBeingPlaced.ExistsInPos(x, y))
                        {
                            var rotatedPos = itemBeingPlaced.GetRotatedPoint(new Vector2Int(x, y));
                            var objectPosition = Room.GetPositionAt(position.Position.x + rotatedPos.x, position.Position.y + rotatedPos.y);
                            objectPosition.Item = itemBeingPlaced;
                        }
                    }
                }
                
                itemBeingPlaced.SetPlacingType(Item.PlacingType.None);
                CurrentMode = ControlMode.None;
            }

            return true;
        }

        private void PlaceItemAtPoint(Vector3 point)
        {
            itemBeingPlaced.transform.position = point;
            itemBeingPlaced.SetPlacingType(Item.PlacingType.NotAvailable);
        }
        
        private Hit PointerRaycast()
        {
            var ray = Camera.ScreenPointToRay(Input.mousePosition);
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