using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ArrayLayout))]
public class CustPropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PrefixLabel(position, label);
		Rect newposition = position;
		newposition.y += 18f;

		var sizeProp = property.FindPropertyRelative("Size");
		int size = sizeProp.intValue;
		Debug.Log("Size: " + size);
		SerializedProperty data = property.FindPropertyRelative("Rows");
		
		for (int j = 0; j < size; j++)
		{
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("Row");
			newposition.height = 18f;
			if (row.arraySize != size)
				row.arraySize = size;
			
			newposition.width = position.width / size;
			
			for (int i = 0; i < size; i++)
			{
				EditorGUI.PropertyField(newposition, row.GetArrayElementAtIndex(i), GUIContent.none);
				newposition.x += newposition.width;
			}

			newposition.x = position.x;
			newposition.y += 18f;
		}
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return 18f * 8;
	}
}
