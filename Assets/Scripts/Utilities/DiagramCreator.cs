using Unity.VisualScripting;
using UnityEngine;

public class DiagramCreator
{
	float[] data;
	int length;
	bool init = false;
	GameObject diagram;
	LineRenderer lineRenderer;

	public void CreateDiagram(float[] data)
	{
		if (init) return;

		this.data = data;
		this.length = data.Length;

		// Create a new GameObject to hold the diagram
		diagram = new GameObject("Diagram");

		// Create a LineRenderer component to draw the diagram
		lineRenderer = diagram.AddComponent<LineRenderer>();

		// Convert the input array of floats into an array of Vector3 objects
		Vector3[] positions = new Vector3[length];
		for (int i = 0; i < length; i++)
		{
			positions[i] = new Vector3(i, data[i], 0);
		}

		// Set the LineRenderer to use the array of Vector3 objects as its positions
		lineRenderer.positionCount = length;
		lineRenderer.SetPositions(positions);

		// Set other properties of the LineRenderer as desired
		lineRenderer.startWidth = 0.1f;
		lineRenderer.endWidth = 0.1f;
		lineRenderer.startColor = Color.red;
		lineRenderer.endColor = Color.red;

		init = true;
	}

	public void UpdateDiagram(float[] newData)
	{
		if (!init) return;

		data = newData;
		length = newData.Length;

		Vector3[] positions = new Vector3[length];
		for (int i = 0; i < length; i++)
		{
			positions[i] = new Vector3(i*0.01f, data[i] * 10000000000000f, 0);
		}

		// Set the LineRenderer to use the array of Vector3 objects as its positions
		lineRenderer.positionCount = length;
		lineRenderer.SetPositions(positions);
	}
}