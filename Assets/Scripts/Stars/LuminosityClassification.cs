using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Luminosity Classification", menuName = "Stars/Luminosity Classification System")]
public class LuminosityClassification : ScriptableObject
{
	public string classificationName;
	public LuminosityClass[] luminosityClasses;

	public LuminosityClass Classify(float bolometricLuminosity, float effectiveTemperature, SpectralClass spectralClass)
	{
		if (spectralClass.letter == "L" || spectralClass.letter == "T" || spectralClass.letter == "Y")
		{
			return Array.Find(luminosityClasses, x => x.className == "V");	// brown dwarfs are main sequence, their mass and luminosity is near-constant
		}
		float bolometricMagnitude = 4.75f - 2.5f * Mathf.Log10(bolometricLuminosity / Universe.solarLuminosity);
		float bolometricCorrection = spectralClass.GetBC(spectralClass.GetRelativeType(effectiveTemperature));
		float absoluteMagnitude = bolometricMagnitude - bolometricCorrection;
		return null;
		//TODO: find way to import ranges for absolute magnitudes
	}
}

[System.Serializable]
public class LuminosityClass
{
	public string className;
	public string prefix;
	public string description;
}
