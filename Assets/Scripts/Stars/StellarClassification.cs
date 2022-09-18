using UnityEngine;

public static class StellarClassification
{
	public static string Classify(float effectiveTemperature)
	{
		SpectralClassification harvard = Resources.Load<SpectralClassification>("Classifications/Harvard");
		string spectral = harvard.ClassifyByTemperature(effectiveTemperature);
		Debug.Log(spectral);
		return "";
	}
}