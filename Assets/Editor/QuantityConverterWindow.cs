using UnityEngine;
using UnityEditor;
using System;

[ExecuteInEditMode]
public class QuantityConverterWindow : EditorWindow
{
	SerializedProperty property;
    public static void Open(SerializedProperty property)
	{
		var window = GetWindow<QuantityConverterWindow>("Quantity Converter");
		window.property = property;
	}

	public void OnGUI()
	{
		EditorGUILayout.BeginVertical();
		EditorGUILayout.LabelField(property.FindPropertyRelative("amount").floatValue.ToString(), Enum.GetName(typeof(Length.Unit), (Length.Unit)property.FindPropertyRelative("Unit").enumValueIndex));
		EditorGUILayout.LabelField("is equal to");
		EditorGUILayout.BeginHorizontal();
		Quantity newQuantity = new Quantity(property.FindPropertyRelative("amount").floatValue, (Length.Unit)property.FindPropertyRelative("Unit").enumValueIndex);
		Length.Unit selectedUnit = newQuantity.Unit;
		EditorGUILayout.LabelField(newQuantity.amount.ToString());
		newQuantity.Convert((Length.Unit)EditorGUILayout.EnumPopup(selectedUnit));
		EditorGUILayout.EndHorizontal();
		if (GUILayout.Button("Store & Close"))
		{
			GetWindow<QuantityConverterWindow>().Close();
		}
		EditorGUILayout.EndVertical();
	}
}
