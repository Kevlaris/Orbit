using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlaceInOrbit))]
public class PlaceInOrbitEditor : Editor
{
	private SerializedProperty centralBodyProp;
	private SerializedProperty distanceProp;
	private SerializedProperty eccentricityProp;
	private SerializedProperty semiMajorAxisProp;
	private SerializedProperty semiMinorAxisProp;
	private SerializedProperty perihelionProp;
	private SerializedProperty aphelionProp;
	private SerializedProperty thetaProp;
	private SerializedProperty phiProp;
	private SerializedProperty initialVelocityProp;
	PlaceInOrbit script;

	private void OnEnable()
	{
		centralBodyProp = serializedObject.FindProperty("centralBody");
		distanceProp = serializedObject.FindProperty("distance");
		eccentricityProp = serializedObject.FindProperty("eccentricity");
		semiMajorAxisProp = serializedObject.FindProperty("semiMajorAxis");
		semiMinorAxisProp = serializedObject.FindProperty("semiMinorAxis");
		perihelionProp = serializedObject.FindProperty("perihelion");
		aphelionProp = serializedObject.FindProperty("aphelion");
		thetaProp = serializedObject.FindProperty("theta");
		phiProp = serializedObject.FindProperty("phi");
		initialVelocityProp = serializedObject.FindProperty("initialVelocity");

		script = (PlaceInOrbit)target;
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		EditorGUILayout.PropertyField(centralBodyProp);
		EditorGUILayout.PropertyField(distanceProp);
		EditorGUILayout.PropertyField(eccentricityProp);
		EditorGUILayout.PropertyField(semiMajorAxisProp);
		EditorGUILayout.PropertyField(semiMinorAxisProp);
		EditorGUILayout.PropertyField(perihelionProp);
		EditorGUILayout.PropertyField(aphelionProp);
		EditorGUILayout.PropertyField(thetaProp);
		EditorGUILayout.PropertyField(phiProp);

		EditorGUILayout.Space();

		if (GUILayout.Button("Calculate values"))
		{
			script.Recalculate();
		}
		if (GUILayout.Button("Set parameters to body"))
		{
			script.SetParameters();
		}

		EditorGUILayout.PropertyField(initialVelocityProp);

		serializedObject.ApplyModifiedProperties();
	}
}
