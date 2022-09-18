using UnityEngine;

[CreateAssetMenu(fileName = "New Luminosity Classification", menuName = "Stars/Luminosity Classification System")]
public class LuminosityClassification : ScriptableObject
{
	public string classificationName;
	public LuminosityClass[] luminosityClasses;

	public void Classify(float bolometricLuminosity, float effectiveTemperature, SpectralClass spectralClass)
	{
		float bolometricMagnitude = 4.75f - 2.5f * Mathf.Log10(bolometricLuminosity / Universe.solarLuminosity);
		float bolometricCorrection = spectralClass.GetBC(spectralClass.GetRelativeType(effectiveTemperature));
		float absoluteMagnitude = bolometricMagnitude - bolometricCorrection;
	}
}

[System.Serializable]
public class LuminosityClass
{
	public string className;
	public string prefix;
	public string description;
}
