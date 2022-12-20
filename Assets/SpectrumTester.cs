using UnityEngine;

public class SpectrumTester : MonoBehaviour
{
	public Star star;
	public int spectrumLength = 100;
	public float[] spectrum;
	DiagramCreator diagram;

	private void Awake()
	{
		diagram = new DiagramCreator();
		diagram.CreateDiagram(new float[1]);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.F))
		{
			Debug.Log("Computing spectrum...");
			spectrum = Spectrum.ComputeStarSpectrum(star.temperature, star.radius, star.absoluteMagnitude, spectrumLength);

			diagram.UpdateDiagram(spectrum);
		}
	}
}
