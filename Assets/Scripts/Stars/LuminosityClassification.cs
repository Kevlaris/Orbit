using System;
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

		float absoluteMagnitude = LuminosityToAbsoluteMagnitude(bolometricLuminosity, effectiveTemperature);

		MagnitudeLoader loader = new MagnitudeLoader();
		loader.LoadCSV();
		int query = loader.FindClass(spectralClass, absoluteMagnitude);
		if (query == -1)
		{
			return null;
		}
		return luminosityClasses[query];
	}
	public LuminosityClass Classify(float absoluteMagnitude, SpectralClass spectralClass)
	{
		if (spectralClass.letter == "L" || spectralClass.letter == "T" || spectralClass.letter == "Y")
		{
			return Array.Find(luminosityClasses, x => x.className == "V");  // brown dwarfs are main sequence, their mass and luminosity is near-constant
		}

		MagnitudeLoader loader = new MagnitudeLoader();
		loader.LoadCSV();
		int query = loader.FindClass(spectralClass, absoluteMagnitude);
		if (query == -1)
		{
			return null;
		}
		return luminosityClasses[query];
	}
	public static float LuminosityToAbsoluteMagnitude(float bolometricLuminosity, float effectiveTemperature)
	{
		SpectralClassification harvard = Resources.Load<SpectralClassification>("Classifications/Harvard");
		SpectralClass spectralClass = harvard.GetClass(effectiveTemperature);

		float bolometricMagnitude = 4.75f - 2.5f * Mathf.Log10(bolometricLuminosity / Universe.solarLuminosity);    //https://astronomy.stackexchange.com/a/39613
		float bolometricCorrection = spectralClass.GetBC(spectralClass.GetRelativeType(effectiveTemperature));
		return bolometricMagnitude - bolometricCorrection;
	}
	public static float AbsoluteMagnitudeToLuminosity(float absoluteMagnitude, float effectiveTemperature)
	{
		SpectralClassification harvard = Resources.Load<SpectralClassification>("Classifications/Harvard");
		SpectralClass spectralClass = harvard.GetClass(effectiveTemperature);

		float bolometricCorrection = spectralClass.GetBC(spectralClass.GetRelativeType(effectiveTemperature));
		float bolometricMagnitude = absoluteMagnitude - bolometricCorrection;

		//return Universe.zeroPointLuminosity * Mathf.Pow(10, -0.4f * bolometricMagnitude);	//https://en.wikipedia.org/wiki/Absolute_magnitude#Bolometric_magnitude
		return Mathf.Pow(10, -(4 * absoluteMagnitude + 4 * bolometricCorrection - 19) / 10) * Universe.solarLuminosity;
	}
}

[System.Serializable]
public class LuminosityClass
{
	public string className;
	public string prefix;
	public string description;
}
