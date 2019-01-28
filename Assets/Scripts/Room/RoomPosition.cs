using Scripts.Items;
using UnityEngine;

namespace Scripts
{
	public class RoomPosition : MonoBehaviour
	{
		public Vector2Int Position;
		public Item Item;
		public GameObject Tile;
		public GameObject HiddenTile;

		public bool IsTaken
		{
			get { return Item != null; }
		}

		public void SetItem(Item item)
		{
			Item = item;
		}

		public void SwitchTile()
		{
			if (Tile != null)
			{
				Tile.SetActive(false);
			}

			if (HiddenTile != null)
			{
				HiddenTile.SetActive(true);
			}
		}

	}
}