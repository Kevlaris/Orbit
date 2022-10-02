using UnityEngine;

public static class StellarClassification
{
	public struct StellarClass
	{
		public SpectralClass SpectralClass;
		public int RelativeType;
		public LuminosityClass LuminosityClass;
	}

	public static StellarClass Classify(float effectiveTemperature, float bolometricLuminosity)
	{
		SpectralClassification harvard = Resources.Load<SpectralClassification>("Classifications/Harvard");
		LuminosityClassification yerkes = Resources.Load<LuminosityClassification>("Classifications/Yerkes");
		SpectralClass spectral = harvard.GetClass(effectiveTemperature);
		int relativeType = spectral.GetRelativeType(effectiveTemperature);
		LuminosityClass luminosityClass = yerkes.Classify(bolometricLuminosity, effectiveTemperature, spectral);

		return new StellarClass()
		{
			SpectralClass = spectral,
			RelativeType = relativeType,
			LuminosityClass = luminosityClass
		};
	}
	public static StellarClass ClassifyByMagnitude(float effectiveTemperature, float absoluteMagnitude)
	{
		SpectralClassification harvard = Resources.Load<SpectralClassification>("Classifications/Harvard");
		LuminosityClassification yerkes = Resources.Load<LuminosityClassification>("Classifications/Yerkes");
		SpectralClass spectral = harvard.GetClass(effectiveTemperature);
		int relativeType = spectral.GetRelativeType(effectiveTemperature);
		LuminosityClass luminosityClass = yerkes.Classify(absoluteMagnitude, spectral);

		return new StellarClass()
		{
			SpectralClass = spectral,
			RelativeType = relativeType,
			LuminosityClass = luminosityClass
		};
	}
	public static string ClassString(StellarClass stellarClass)
	{
		if (stellarClass.LuminosityClass == null && stellarClass.SpectralClass != null)
		{
			return stellarClass.SpectralClass.GetType(stellarClass.RelativeType) + "?";
		}
		else if (stellarClass.LuminosityClass != null && stellarClass.SpectralClass == null)
		{
			return "?" + stellarClass.LuminosityClass.className;
		}
		else if (stellarClass.LuminosityClass != null && stellarClass.SpectralClass != null)
		{
			if (stellarClass.LuminosityClass.prefix != null && stellarClass.LuminosityClass.prefix != "" && stellarClass.LuminosityClass.prefix != " ")
			{
				return stellarClass.LuminosityClass.prefix + stellarClass.SpectralClass.GetType(stellarClass.RelativeType);
			}
			else
			{
				return stellarClass.SpectralClass.GetType(stellarClass.RelativeType) + stellarClass.LuminosityClass.className;
			}
		}
		else
		{
			return "?";
		}
	}
}