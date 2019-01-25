using UnityEngine;

[System.Serializable]
public class ArrayLayout
{
	private const int SIZE = 5;

	[SerializeField] public int Size = 5;
	[SerializeField] public rowData[] Rows = new rowData[SIZE];
}

[System.Serializable]
public struct rowData{
	[SerializeField] public bool[] Row;
}