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

        private Item itemBeingPlaced;
        
        private void Update()
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
                TryPlacingAtPosition(position);
            }
            
            var room = raycast.hit.rigidbody.GetComponent<Room>();
            if (room != null)
            {
                PlaceItemAtPoint(raycast.hit.point);
            }
        }

        private void TryPlacingAtPosition(RoomPosition position)
        {
            // Object can be placed
            itemBeingPlaced.SetPosition(position);
            
            if (position.IsTaken)
            {
                // Position already taken
                // TODO: Check all other positions according to object size
                itemBeingPlaced.SetPlacingType(Item.PlacingType.NotAvailable);
                return;
            }
            
            itemBeingPlaced.SetPlacingType(Item.PlacingType.Available);

            if (Input.GetMouseButtonDown(0))
            {
                // Place item
                position.Item = itemBeingPlaced;
                itemBeingPlaced.SetPlacingType(Item.PlacingType.None);
                CurrentMode = ControlMode.None;
            }
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