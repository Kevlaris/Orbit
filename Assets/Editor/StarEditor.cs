using UnityEditor;
using UnityEngine;

[CanEditMultipleObjects]
[CustomEditor(typeof(Star))]
public class StarEditor : Editor
{
	SerializedProperty surfaceGravity, solarRadius, temperature, solarLuminosity, absoluteMagnitude, chromaticity;
	StellarClassification.StellarClass stellarClass;
	bool useMagnitude = false;
	float luminanceValue;

	private void OnEnable()
	{
		serializedObject.Update();
		surfaceGravity = serializedObject.FindProperty("surfaceGravity");
		solarRadius = serializedObject.FindProperty("solarRadius");
		temperature = serializedObject.FindProperty("temperature");
		solarLuminosity = serializedObject.FindProperty("solarLuminosity");
		absoluteMagnitude = serializedObject.FindProperty("absoluteMagnitude");
		chromaticity = serializedObject.FindProperty("chromaticity");
		if (useMagnitude)
		{
			luminanceValue = absoluteMagnitude.floatValue;
		}
		else
		{
			luminanceValue = solarLuminosity.floatValue;
		}
		GetClass();
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();
		EditorGUILayout.BeginVertical();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(temperature);
		EditorGUILayout.LabelField(StellarClassification.ClassString(stellarClass));
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.PropertyField(chromaticity);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(solarRadius);
		if (GUILayout.Button("Scale"))
		{
			ScaleStar();
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.PropertyField(surfaceGravity);
		EditorGUILayout.LabelField("m/s" + '\u00B2');
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.BeginHorizontal();
		if (useMagnitude)
		{
			luminanceValue = EditorGUILayout.FloatField("Absolute Magnitude", luminanceValue);
			absoluteMagnitude.floatValue = luminanceValue;
			solarLuminosity.floatValue = LuminosityClassification.AbsoluteMagnitudeToLuminosity(absoluteMagnitude.floatValue, temperature.floatValue) / Universe.solarLuminosity;

		}
		else
		{
			luminanceValue = EditorGUILayout.FloatField("Solar Luminosity", luminanceValue);
			solarLuminosity.floatValue = luminanceValue;
			absoluteMagnitude.floatValue = LuminosityClassification.LuminosityToAbsoluteMagnitude(solarLuminosity.floatValue * Universe.solarLuminosity, temperature.floatValue);
		}
		if (GUILayout.Button("Swap"))
		{
			if (useMagnitude)
			{
				luminanceValue = LuminosityClassification.AbsoluteMagnitudeToLuminosity(absoluteMagnitude.floatValue, temperature.floatValue) / Universe.solarLuminosity;
			}
			else
			{
				luminanceValue = LuminosityClassification.LuminosityToAbsoluteMagnitude(solarLuminosity.floatValue * Universe.solarLuminosity, temperature.floatValue);
			}
			useMagnitude = !useMagnitude;
		}
		EditorGUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
		serializedObject.ApplyModifiedProperties();
		GetClass();
	}

	void ScaleStar()
	{
		((Star)target).ScaleParticleSystems(new Quantity(solarRadius.floatValue * Universe.solarRadius, Length.Unit.km));
	}

	void GetClass()
	{
		if (useMagnitude)
		{
			stellarClass = StellarClassification.ClassifyByMagnitude(temperature.floatValue, luminanceValue);
		}
		else
		{
			stellarClass = StellarClassification.Classify(temperature.floatValue, luminanceValue * Universe.solarLuminosity);
		}
	}
}
