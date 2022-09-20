using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class MagnitudeLoader
{
	bool init = false;
	TextAsset csvFile;
	readonly char lineSeparator = '\n';
	readonly char fieldSeparator = ';';
	Dictionary<string, string[,]> magnitudes = new Dictionary<string, string[,]>();

	public void LoadCSV()
	{
		magnitudes = new Dictionary<string, string[,]>();
		csvFile = Resources.Load<TextAsset>("Classifications/magnitudes");
		string[] lines = csvFile.text.Split(lineSeparator);

		for (int i = 1; i < lines.Length; i++)
		{
			string line = lines[i];

			string[] fields = line.Split(fieldSeparator);

			var key = fields[0];
			string[,] classMagnitudes = new string[8, 2];

			for (int j = 0; j < 8; j++)
			{
				for (int k = 0; k < 2; k++)
				{
					classMagnitudes[j, k] = fields[j + k + 1];
				}
			}
			magnitudes.Add(key, classMagnitudes);
		}
		init = true;
	}

	/// <summary>
	/// Find luminosity class based on absolute magnitude
	/// </summary>
	/// <param name="spectralClass">Spectral class of star</param>
	/// <param name="absoluteMagnitude">Absolute magnitude of star</param>
	/// <returns>Index of Luminosity Class in Yerkes system (e.g. II is 2, 0 is 0)</returns>
	public int FindClass(SpectralClass spectralClass, float absoluteMagnitude)
	{
		if (!init)
		{
			Debug.LogError("Loader has not been initialised before query");
			return -2;
		}
		magnitudes.TryGetValue(spectralClass.letter.Trim(), out string[,] classMagnitudes);
		if (classMagnitudes == null)
		{
			Debug.LogError("Spectral class not found in magnitudes.csv", csvFile);
			return -1;
		}
		List<int> possibleMatches = new List<int>();
		for (int i = 0; i < 8; i++)
		{
			LuminosityClassification yerkes = Resources.Load<LuminosityClassification>("Classifications/Yerkes");
			Debug.Log(i + ": " + yerkes.luminosityClasses[8 - i - 1].className);
			if (classMagnitudes[i,0] == "x" || classMagnitudes[i, 1] == "x")
			{
				continue;
			}
			else
			{
				float.TryParse(classMagnitudes[i, 0], out float mag1);
				float mag2;
				if (classMagnitudes[i,1] == "Infinity")
				{
					mag2 = float.PositiveInfinity;
				}
				else
				{
					float.TryParse(classMagnitudes[i, 1], out mag2);
				}				

				if (absoluteMagnitude > mag1 && absoluteMagnitude < mag2)	// Mv is in a range
				{
					Debug.Log(spectralClass.letter + ": " + mag1 + "<" + absoluteMagnitude + "<" + mag2);
					Debug.Log("i = " + i);
					Debug.Log("type = " + (8 - i - 1));
					return 8 - i - 1;
				}
				else if (absoluteMagnitude == mag1)	// Mv is the lower limit
				{
					if (i == 7)	// if 0
					{
						return 8 - i - 1;
					}
					else
					{
						possibleMatches.Add(i);
					}
				}
				else if (absoluteMagnitude == mag2)	// Mv is the upper limit
				{
					if (i == 0)	// if VII
					{
						return 8 - i - 1;
					}
					else
					{
						possibleMatches.Add(i);
					}
				}
			}
		}

		if (possibleMatches.Count == 1)
		{
			return possibleMatches[0];
		}
		else if (possibleMatches.Count == 0)
		{
			Debug.LogWarning("The luminosity class of a class " + spectralClass.className + " star with an absolute magnitude of " + absoluteMagnitude + " could not be found!");
			return -1;
		}
		else
		{
			if (possibleMatches.Contains(2))
			{
				return 5;	// assume star is main-sequenced
			}
			else
			{
				return possibleMatches[0];
			}
		}
	}
}