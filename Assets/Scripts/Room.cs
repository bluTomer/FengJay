using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public RoomPosition PositionPrefab;
    public Vector2Int Size;
    public Vector2 Resolution;
    public RoomPosition[,] Positions;

    private Transform parent;

    [ContextMenu("Setup")]
    public void SetupNewRoom()
    {
        if (parent != null)
        {
            Destroy(parent.gameObject);
        }

        parent = new GameObject("Positions").transform;
        parent.SetParent(transform);
        parent.localPosition = Vector3.zero;
        
        Positions = new RoomPosition[Size.x, Size.y];

        for (int x = 0; x < Size.x; x++)
        {
            for (int y = 0; y < Size.y; y++)
            {
                var position = Instantiate(PositionPrefab, transform);
                position.Position = new Vector2Int(x, y);
                
                position.transform.position = new Vector3(x * Resolution.x, y * Resolution.y, transform.position.z);
                Positions[x, y] = position;
            }
        }
    }
}
