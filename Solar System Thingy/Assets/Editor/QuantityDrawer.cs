using UnityEditor;
using UnityEngine;
using System;

[CustomPropertyDrawer(typeof(Quantity))]
public class QuantityDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		int unitNum = property.FindPropertyRelative("Unit").enumValueIndex;
		var centeredLabel = GUI.skin.GetStyle("Label");
		centeredLabel.alignment = TextAnchor.MiddleCenter;

		EditorGUI.BeginProperty(position, label, property);

		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		Rect amountRect = new Rect(position.x, position.y, position.width - 105, position.height);
		Rect unitRect = new Rect(position.x + amountRect.width + 5, position.y, 30, position.height);
		if (!property.FindPropertyRelative("isStatic").boolValue)
		{
			Rect upButtonRect = new Rect(position.x + amountRect.width + unitRect.width + 5, position.y, 30, position.height);
			Rect downButtonRect = new Rect(position.x + amountRect.width + unitRect.width + upButtonRect.width + 5, position.y, 30, position.height);

			GUIContent upArrow = new GUIContent(Resources.Load<Texture>("Textures/up"));
			if (unitNum + 1 < Enum.GetNames(typeof(Length.Unit)).Length)
			{
				if (GUI.Button(upButtonRect, upArrow))
				{
					Quantity newQuantity = Length.Convert(new Quantity(property.FindPropertyRelative("amount").floatValue, (Length.Unit)unitNum), (Length.Unit)unitNum + 1);
					property.FindPropertyRelative("amount").floatValue = newQuantity.amount;
					property.FindPropertyRelative("Unit").enumValueIndex = unitNum + 1;
				}
			}
			else
			{
				GUI.Label(upButtonRect, upArrow);
			}

			GUIContent downArrow = new GUIContent(Resources.Load<Texture>("Textures/down"));
			if (unitNum - 1 >= 0)
			{
				if (GUI.Button(downButtonRect, downArrow))
				{
					Quantity newQuantity = Length.Convert(new Quantity(property.FindPropertyRelative("amount").floatValue, (Length.Unit)unitNum), (Length.Unit)unitNum - 1);
					property.FindPropertyRelative("amount").floatValue = newQuantity.amount;
					property.FindPropertyRelative("Unit").enumValueIndex = unitNum - 1;
				}
			}
			else
			{
				GUI.Label(downButtonRect, downArrow);
			}
		}		

		EditorGUI.PropertyField(amountRect, property.FindPropertyRelative("amount"), GUIContent.none);
		EditorGUI.LabelField(unitRect, Enum.GetName(typeof(Length.Unit), (Length.Unit)unitNum), centeredLabel);

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}