using System;
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
				EditorGUILayout.LabelField(new GUIContent(StellarClassification.ClassString(stellarClass), stellarClass.SpectralClass.className.Split('(', StringSplitOptions.RemoveEmptyEntries)[1].TrimEnd(')')));
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.PropertyField(chromaticity);

			EditorGUILayout.BeginHorizontal();
				EditorGUILayout.PropertyField(solarRadius);
				if (GUILayout.Button(new GUIContent("Scale", "Scale the star's particles")))
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
		
				if (GUILayout.Button(new GUIContent("Swap", "Swap between Luminosity and Absolute Magnitude")))
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

			EditorGUILayout.Separator();

			if (((Star)target).GetParticleObject() != null)
			{
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button(new GUIContent("Save Particles")))
				{
					SaveParticles();
				}
				if (GUILayout.Button("Reload"))
				{
					ReloadParticles();
				}
				if (GUILayout.Button("Unload"))
				{
					UnloadParticles();
				}
				EditorGUILayout.EndHorizontal();
			}
			else
			{
				if (GUILayout.Button("Load Particles"))
				{
					LoadParticles();
				}
			}

			EditorGUILayout.EndVertical();
		serializedObject.ApplyModifiedProperties();
		GetClass();
	}

	void ScaleStar()
	{
		((Star)target).ScaleParticleSystems(solarRadius.floatValue);
	}

	void GetClass()
	{
		/*
		if (useMagnitude)
		{
			stellarClass = StellarClassification.ClassifyByMagnitude(temperature.floatValue, luminanceValue);
		}
		else
		{
			stellarClass = StellarClassification.Classify(temperature.floatValue, luminanceValue * Universe.solarLuminosity);
		}
		*/

		stellarClass = ((Star)target).GetClass();
	}

	void SaveParticles()
	{
		string localPath = "Assets/Resources/Stars/Particles/" + StellarClassification.ClassString(stellarClass) + ".prefab";
		localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
		GameObject particleObject = ((Star)target).GetParticleObject();
		if (!particleObject) return;
		PrefabUtility.SaveAsPrefabAsset(((Star)target).transform.GetChild(1).gameObject, localPath, out bool success);
		if (!success) Debug.LogWarning("Can't save particle system");
	}
	void ReloadParticles()
	{
		((Star)target).LoadParticleSystem();
	}
	void UnloadParticles()
	{
		((Star)target).UnloadParticleSystem();
	}
	void LoadParticles()
	{
		((Star)target).CalibrateParticleSystems();
	}
}
