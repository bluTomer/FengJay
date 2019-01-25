using UnityEngine;

[System.Serializable]
public class Bool5x5
{
	private const int SIZE = 5;

	[SerializeField] public int Size = SIZE;
	[SerializeField] public rowData[] Rows = new rowData[SIZE];
}

[System.Serializable]
public struct rowData{
	[SerializeField] public bool[] Row;
}