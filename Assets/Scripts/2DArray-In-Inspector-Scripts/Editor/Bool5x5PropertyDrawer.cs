using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Bool5x5))]
public class Bool5x5PropertyDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.PrefixLabel(position, label);
		Rect newposition = position;
		newposition.y += 18f;

		int size = property.FindPropertyRelative("Size").intValue;
		SerializedProperty data = property.FindPropertyRelative("Rows");
		
		for (int j = 0; j < size; j++)
		{
			SerializedProperty row = data.GetArrayElementAtIndex(j).FindPropertyRelative("Row");
			newposition.height = 18f;
			if (row.arraySize != size)
				row.arraySize = size;
			
			newposition.width = 18f;
			
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
		int size = property.FindPropertyRelative("Size").intValue;
		return 18f * (size + 1);
	}
}
