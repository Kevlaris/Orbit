using UnityEngine;

[ExecuteInEditMode]
public class OrbitDebugDisplay : MonoBehaviour
{

	public int numSteps = 1000;
	public float timeStep = 0.1f;
	public bool usePhysicsTimeStep;

	public bool relativeToBody;
	public GravityBody centralBody;
	public float width = 100;
	public bool useThickLines;

	void Start()
	{
		if (Application.isPlaying)
		{
			HideOrbits();
		}
	}
	void Update()
	{
		if (!Application.isPlaying)
		{
			DrawOrbits();
		}
	}

	void DrawOrbits()
	{
		GravityBody[] bodies = FindObjectsOfType<GravityBody>();
		var virtualBodies = new VirtualBody[bodies.Length];
		var drawPoints = new Vector3[bodies.Length][];
		int referenceFrameIndex = 0;
		Vector3 referenceBodyInitialPosition = Vector3.zero;

		// Initialize virtual bodies (don't want to move the actual bodies)
		for (int i = 0; i < virtualBodies.Length; i++)
		{
			virtualBodies[i] = new VirtualBody(bodies[i]);
			drawPoints[i] = new Vector3[numSteps];

			if (bodies[i] == centralBody && relativeToBody)
			{
				referenceFrameIndex = i;
				referenceBodyInitialPosition = virtualBodies[i].position;
			}
		}

		// Simulate
		for (int step = 0; step < numSteps; step++)
		{
			Vector3 referenceBodyPosition = (relativeToBody) ? virtualBodies[referenceFrameIndex].position : Vector3.zero;
			// Update velocities
			for (int i = 0; i < virtualBodies.Length; i++)
			{
				virtualBodies[i].velocity += CalculateAcceleration(i, virtualBodies) * timeStep;
			}
			// Update positions
			for (int i = 0; i < virtualBodies.Length; i++)
			{
				Vector3 newPos = virtualBodies[i].position + virtualBodies[i].velocity * timeStep;
				virtualBodies[i].position = newPos;
				if (relativeToBody)
				{
					var referenceFrameOffset = referenceBodyPosition - referenceBodyInitialPosition;
					newPos -= referenceFrameOffset;
				}
				if (relativeToBody && i == referenceFrameIndex)
				{
					newPos = referenceBodyInitialPosition;
				}

				drawPoints[i][step] = newPos;
			}
		}

		// Draw paths
		for (int bodyIndex = 0; bodyIndex < virtualBodies.Length; bodyIndex++)
		{
			var pathColour = bodies[bodyIndex].gameObject.GetComponentInChildren<MeshRenderer>().sharedMaterial.color; //

			if (useThickLines)
			{
				var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
				lineRenderer.enabled = true;
				lineRenderer.positionCount = drawPoints[bodyIndex].Length;
				lineRenderer.SetPositions(drawPoints[bodyIndex]);
				lineRenderer.startColor = pathColour;
				lineRenderer.endColor = pathColour;
				lineRenderer.widthMultiplier = width;
			}
			else
			{
				for (int i = 0; i < drawPoints[bodyIndex].Length - 1; i++)
				{
					Debug.DrawLine(drawPoints[bodyIndex][i], drawPoints[bodyIndex][i + 1], pathColour);
				}

				// Hide renderer
				var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
				if (lineRenderer)
				{
					lineRenderer.enabled = false;
				}
			}
		}
	}

	Vector3 CalculateAcceleration(int i, VirtualBody[] virtualBodies)
	{
		Vector3 acceleration = Vector3.zero;
		for (int j = 0; j < virtualBodies.Length; j++)
		{
			if (i == j)
			{
				continue;
			}
			var body1 = virtualBodies[i];
			var body2 = virtualBodies[j];

			Vector3 direction = body2.position - body1.position;
			float distance = direction.magnitude;
			Vector3 unitVector = direction / distance;
			float force = Universe.gravitationalConstant * body2.mass / Mathf.Pow(distance, 2);
			//Debug.Log(gravForce);
			acceleration += force * unitVector;
		}
		return acceleration;
	}

	void HideOrbits()
	{
		GravityBody[] bodies = FindObjectsOfType<GravityBody>();

		// Draw paths
		for (int bodyIndex = 0; bodyIndex < bodies.Length; bodyIndex++)
		{
			var lineRenderer = bodies[bodyIndex].gameObject.GetComponentInChildren<LineRenderer>();
			if (lineRenderer != null) lineRenderer.positionCount = 0;
		}
	}

	void OnValidate()
	{
		if (usePhysicsTimeStep)
		{
			timeStep = Universe.physicsTimeStep;
		}
	}

	class VirtualBody
	{
		public Vector3 position;
		public Vector3 velocity;
		public float mass;

		public VirtualBody(GravityBody body)
		{
			position = body.transform.position;
			velocity = body.initialVelocity * (Mathf.PI * 10);
			mass = body.mass;
		}
	}
}