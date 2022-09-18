using UnityEngine;

[CreateAssetMenu(fileName = "New Luminosity Classification", menuName = "Stars/Luminosity Classification System")]
public class LuminosityClassification : ScriptableObject
{
	public string classificationName;
	public LuminosityClass[] luminosityClasses;
}

[System.Serializable]
public class LuminosityClass
{
	public string className;
	public string prefix;
	public string description;
}
