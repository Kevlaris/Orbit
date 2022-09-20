using UnityEngine;

public static class StellarClassification
{
	public static string Classify(float effectiveTemperature, float bolometricLuminosity)
	{
		SpectralClassification harvard = Resources.Load<SpectralClassification>("Classifications/Harvard");
		LuminosityClassification yerkes = Resources.Load<LuminosityClassification>("Classifications/Yerkes");
		SpectralClass spectral = harvard.GetClass(effectiveTemperature);
		LuminosityClass luminosityClass = yerkes.Classify(bolometricLuminosity, effectiveTemperature, spectral);

		Debug.Log("spectral: " + spectral.GetType(effectiveTemperature));
		Debug.Log("luminosity: " + luminosityClass.className);
		return "";
	}
}