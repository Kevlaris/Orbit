using UnityEngine;

// Completely generated by ChatGPT on 2022.12.14.
public class OrbitVisualiser : MonoBehaviour
{
	// The semi-major and semi-minor axes of the ellipse
	public Quantity a, b;

	// The angle of the ellipse relative to the x-axis - possibly phi
	public float angle;

	// The velocity of the ellipse
	public Vector3 velocity;

	// The number of points to calculate on the ellipse
	public int pointCount = 1000;

	void OnDrawGizmos()
	{
		// Calculate the points on the ellipse
		Vector3[] points = CalculateEllipsePoints(a, b, angle, velocity);

		// Draw a line through the points on the ellipse
		Gizmos.color = Color.red;
		for (int i = 0; i < points.Length - 1; i++)
		{
			Gizmos.DrawLine(points[i], points[i + 1]);
		}
	}

	Vector3[] CalculateEllipsePoints(Quantity a, Quantity b, float angle, Vector3 velocity)
	{
		// The points on the ellipse
		Vector3[] points = new Vector3[pointCount];

		for (int i = 0; i < pointCount; i++)
		{
			// Calculate the angle for the current point
			float theta = 2 * Mathf.PI * i / (pointCount - 1);

			// Calculate the x, y, and z coordinates of the point on the ellipse
			Quantity x = new Quantity(a * Mathf.Cos(theta) * Mathf.Cos(angle), Length.Unit.m);
			Quantity y = new Quantity(a * Mathf.Cos(theta) * Mathf.Sin(angle), Length.Unit.m);
			Quantity z = new Quantity(b * Mathf.Sin(theta), Length.Unit.m);

			// Set the point on the ellipse
			//points[i] = new Vector3(Length.ConvertToWorld(x), Length.ConvertToWorld(y), Length.ConvertToWorld(z));
			points[i] = new Vector3(x, y, z);
		}

		// Transform the points to their correct position and orientation in 3D space
		TransformEllipsePoints(points, a, b, angle, velocity);

		return points;
	}

	private void TransformEllipsePoints(Vector3[] points, float a, float b, float angle, Vector3 velocity)
	{
		// The center of the ellipse
		Vector3 center = new Vector3(velocity.x, velocity.y, 0);

		// The rotation matrix for the ellipse
		Matrix4x4 rotation = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), Vector3.one);

		// Transform each point on the ellipse
		for (int i = 0; i < points.Length; i++)
		{
			// Translate the point to the center of the ellipse
			points[i] -= center;

			// Rotate the point according to the ellipse's orientation
			points[i] = rotation.MultiplyPoint(points[i]);

			// Translate the point back to its original position
			points[i] += center;

			Quantity x = new Quantity(points[i].x, Length.Unit.m);
			Quantity y = new Quantity(points[i].y, Length.Unit.m);
			Quantity z = new Quantity(points[i].z, Length.Unit.m);

			points[i].x = Length.ConvertToWorld(x);
			points[i].y = Length.ConvertToWorld(y);
			points[i].z = Length.ConvertToWorld(z);
		}
	}
}