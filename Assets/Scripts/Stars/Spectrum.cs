using System;
using UnityEngine;

public class Spectrum
{
	public static float[] ComputeStarSpectrum(float temperature, float radius, float absoluteMagnitude, int length = 100)
	{
		// The constant of Stefan-Boltzmann's Law, in W/m^2/K^4
		const float STEFAN_BOLTZMANN_CONSTANT = 5.67e-8f;

		// Calculate the total power emitted by the star
		//float totalPower = 4 * Mathf.PI * radius * radius * STEFAN_BOLTZMANN_CONSTANT * temperature * temperature * temperature * temperature;

		// Calculate the scaling factor based on the absolute magnitude
		float scalingFactor = Mathf.Pow(10, -0.4f * absoluteMagnitude);

		// Calculate the blackbody spectrum of the star
		float[] spectrum = new float[length];
		for (int i = 0; i < spectrum.Length; i++)
		{
			// Calculate the wavelength of the current band in the spectrum
			float wavelength = 0.1f * i;

			// Calculate the intensity of the current band in the spectrum
			spectrum[i] = 2 * Mathf.PI * STEFAN_BOLTZMANN_CONSTANT / (Mathf.Pow(wavelength, 4)) * (1 / (Mathf.Exp(143877 / (temperature * wavelength)) - 1));

			// Scale the intensity by the absolute magnitude
			spectrum[i] *= scalingFactor;

			if (float.IsNaN(spectrum[i]) || float.IsInfinity(spectrum[i]))
			{
				spectrum[i] = 0;
			}
		}
		return spectrum;
	}
}