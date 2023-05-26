using UnityEngine;

public class OrbitVisualiser : MonoBehaviour
{
	public CelestialBody centralBody;
	public float initialDistance;
	public Vector3 initialVelocity;
	public int segments = 100;

	private void OnDrawGizmos()
	{
		if (centralBody == null) centralBody = GetComponent<CelestialBody>();
		if (centralBody == null) return;

		float centralBodyMass = centralBody.mass;
		float gravitationalConstant = Universe.gravitationalConstant;

		Gizmos.color = Color.white;

		float angleStep = 2f * Mathf.PI / segments;
		float angle = 0f;

		Vector3 centerPosition = centralBody.transform.position;

		for (int i = 0; i < segments; i++)
		{
			float x = Mathf.Cos(angle) * initialDistance;
			float z = Mathf.Sin(angle) * initialDistance;
			Vector3 point1 = centerPosition + new Vector3(x, 0f, z);

			// Calculate the semi-major and semi-minor axes based on the initial distance
			float semiMajorAxis = initialDistance;
			float semiMinorAxis = CalculateSemiMinorAxis(semiMajorAxis, initialDistance);

			// Calculate the position around the ellipse
			float posX = centerPosition.x + Mathf.Cos(angle) * semiMajorAxis;
			float posY = centerPosition.y;
			float posZ = centerPosition.z + Mathf.Sin(angle) * semiMinorAxis;
			Vector3 point2 = new Vector3(posX, posY, posZ);

			// Calculate the orbital velocity at the current position
			Vector3 velocity = CalculateOrbitalVelocity(point2 - centerPosition, centralBodyMass, gravitationalConstant);

			// Apply the initial velocity to the position
			point2 += velocity * Time.deltaTime;

			Gizmos.DrawLine(point1, point2);

			angle += angleStep;
		}
	}

	private float CalculateSemiMinorAxis(float semiMajorAxis, float distance)
	{
		return Mathf.Sqrt(Mathf.Pow(semiMajorAxis, 2) - Mathf.Pow(distance, 2));
	}

	private Vector3 CalculateOrbitalVelocity(Vector3 radiusVector, float centralBodyMass, float gravitationalConstant)
	{
		float radius = radiusVector.magnitude;
		float velocityMagnitude = Mathf.Sqrt(gravitationalConstant * centralBodyMass / radius);
		return Vector3.Cross(Vector3.up, radiusVector.normalized) * velocityMagnitude;
	}
}
