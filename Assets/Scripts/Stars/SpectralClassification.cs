using UnityEngine;

[CreateAssetMenu(fileName = "New Spectral Classification", menuName = "Stars/Spectral Classification System")]
public class SpectralClassification : ScriptableObject
{
	public string classificationName;
	public SpectralClass[] spectralClasses;
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
	public Color chromaticity;  //chromaticity of object in CIE D65 space

	public int[] GetTypes()
	{
		int[] types = new int[numberOfTypes];
		for (int i = 0; i < numberOfTypes; i++)
		{
			types[i] = firstType + i;
		}
		return types;
	}
}