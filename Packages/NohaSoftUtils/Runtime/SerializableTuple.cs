using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace NohaSoftware.Utilities
{
	/// <summary>
	/// Equivalent of a <see cref="System.Tuple{T1, T2}"/>
	/// </summary>
	[Serializable]
	public class SerializableTuple<T1, T2>
	{
		public T1 Key;
		public T2 Value;

		public SerializableTuple(T1 key, T2 value)
		{
			Key = key;
			Value = value;
		}

		public override string ToString()
		{
			return $"{Key}: {Value}";
		}

		public static implicit operator SerializableTuple<T1, T2>(Tuple<T1, T2> tuple)
		{
			return new SerializableTuple<T1, T2>(tuple.Item1, tuple.Item2);
		}
		public static implicit operator Tuple<T1, T2>(SerializableTuple<T1, T2> serializableTuple)
		{
			return new Tuple<T1, T2>(serializableTuple.Key, serializableTuple.Value);
		}
		public static explicit operator KeyValuePair<T1, T2>(SerializableTuple<T1, T2> serializableTuple)
		{
			return new KeyValuePair<T1, T2>(serializableTuple.Key, serializableTuple.Value);
		}
	}

#if UNITY_EDITOR
	[CustomPropertyDrawer(typeof(SerializableTuple<,>))]
	public class SerializableTuplePropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginProperty(position, label, property);

			// Indent the content by the standard amount
			position = EditorGUI.IndentedRect(position);

			// Calculate the height of each element in the property
			float lineHeight = EditorGUIUtility.singleLineHeight;
			float spacing = EditorGUIUtility.standardVerticalSpacing;

			// Draw the key and value fields
			EditorGUI.PropertyField(new Rect(position.x, position.y, position.width / 2 - spacing, lineHeight), property.FindPropertyRelative("Key"), GUIContent.none);
			EditorGUI.PropertyField(new Rect(position.x + position.width / 2, position.y, position.width / 2 - spacing, lineHeight), property.FindPropertyRelative("Value"), GUIContent.none);

			EditorGUI.EndProperty();
		}
	}
#endif

	public static class SerializableTupleExtensions
	{
		public static SerializableTuple<T1, T2> GetElement<T1, T2>(this IEnumerable<SerializableTuple<T1, T2>> serializableTuples, T1 key) where T1 : IEquatable<T1>
		{
			var query = serializableTuples.Where(sTuple => sTuple.Key.Equals(key));
			if (query.Count() > 0) return query.First();
			else return null;
		}
		public static bool ContainsKey<T1, T2>(this IEnumerable<SerializableTuple<T1, T2>> serializableTuples, T1 key) where T1 : IEquatable<T1>
		{
			return serializableTuples.GetElement(key) != null;
		}
	}
}