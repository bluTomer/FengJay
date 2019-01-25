using UnityEngine;

public class RoomPosition : MonoBehaviour
{
	public Vector2Int Position;
	public Item Item;

	public bool IsTaken { get { return Item != null; } }
}
