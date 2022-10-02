using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spectral Classification", menuName = "Stars/Spectral Classification System")]
public class SpectralClassification : ScriptableObject
{
	public string classificationName;
	public SpectralClass[] spectralClasses;

	public string ClassifyByTemperature(float effectiveTemperature)
	{
		SpectralClass[] possibleMatches = Array.FindAll(spectralClasses, x =>
		(effectiveTemperature > x.temperature[0]) && (effectiveTemperature < x.temperature[1]));

		if (possibleMatches.Length == 0)
		{
			SpectralClass a = Array.Find(spectralClasses, x => x.temperature[0] == effectiveTemperature);
			SpectralClass b = Array.Find(spectralClasses, x => x.temperature[1] == effectiveTemperature);
			if (a == null)
			{
				return b.className;
			}
			else if (b == null)
			{
				return a.className;
			}
			else
			{
				SpectralClass[] matches = new SpectralClass[] { a, b };
				return a.letter + "-" + b.letter;
			}
		}
		else if (possibleMatches.Length == 1)
		{
			return possibleMatches[0].GetType(effectiveTemperature);
		}
		return null;
	}
	public SpectralClass GetClass(float effectiveTemperature)
	{
		SpectralClass[] possibleMatches = Array.FindAll(spectralClasses, x =>
		(effectiveTemperature > x.temperature[0]) && (effectiveTemperature < x.temperature[1]));

		if (possibleMatches.Length == 0)
		{
			SpectralClass a = Array.Find(spectralClasses, x => x.temperature[0] == effectiveTemperature);
			SpectralClass b = Array.Find(spectralClasses, x => x.temperature[1] == effectiveTemperature);
			if (a == null)
			{
				return b;
			}
			else if (b == null)
			{
				return a;
			}
			else
			{
				//SpectralClass[] matches = new SpectralClass[] { a, b };
				return a;
			}
		}
		else if (possibleMatches.Length == 1)
		{
			return possibleMatches[0];
		}
		return null;
	}
}

[System.Serializable]
public class SpectralClass
{
	public string className;
	public string letter;
	public int numberOfTypes = 10;
	public int firstType = 0;
	public int[] temperature = new int[2];
	public float[] solarMass = new float[2];
	public float[] solarRadii = new float[2];
	public float[] bolometricLuminosity = new float[2];
	public float[] bolometricCorrection = new float[2];
	[Obsolete] public Color chromaticity;  //chromaticity of object in CIE D65 space
	[Range(0,360)] public float hue;

	public int[] GetTypes()
	{
		int[] types = new int[numberOfTypes];
		for (int i = 0; i < numberOfTypes; i++)
		{
			types[i] = firstType + i;
		}
		return types;
	}
	public string GetType(float effectiveTemperature)
	{
		int totalTemperature = temperature[1] - temperature[0];
		float localTemperature = effectiveTemperature - temperature[0];
		float typeTemperature = totalTemperature / numberOfTypes;
		int index = -1;
		for (int i = 0; i < numberOfTypes; i++)
		{
			if (localTemperature > (numberOfTypes - i) * typeTemperature)
			{
				index = i-1;
				break;
			}
		}
		int type = firstType + index;
		return letter + type;
	}
	public string GetType(int relativeType)
	{
		int type = firstType + relativeType;
		return letter + type;
	}
	public int GetRelativeType(float effectiveTemperature)
	{
		int totalTemperature = temperature[1] - temperature[0];
		float localTemperature = effectiveTemperature - temperature[0];
		float typeTemperature = totalTemperature / numberOfTypes;
		int index = 0;
		for (int i = 0; i < numberOfTypes; i++)
		{
			if (localTemperature > (numberOfTypes - i) * typeTemperature)
			{
				index = i - 1;
				break;
			}
		}
		return index;
	}
	/// <summary>
	/// Calculates BCv of star of spectral type
	/// </summary>
	/// <param name="relativeType">Relative spectral type number of star (e.g. O2 is 0; A0 is 0)</param>
	/// <returns>V-band bolometric correction</returns>
	public float GetBC(int relativeType)
	{
		if (bolometricCorrection.Length == 0)
		{
			Debug.LogWarning("This spectral class does not have bolometric correction specified: " + className);
			return float.NegativeInfinity;
		}
		else if (bolometricCorrection.Length == 1)
		{
			return bolometricCorrection[0];
		}
		else
		{
			float totalBC = bolometricCorrection[1] - bolometricCorrection[0];
			float typeBC = totalBC / numberOfTypes;
			return bolometricCorrection[0] + relativeType * typeBC;
		}
	}
}