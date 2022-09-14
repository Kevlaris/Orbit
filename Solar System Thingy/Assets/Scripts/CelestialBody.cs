using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class CelestialBody : MonoBehaviour
{
    public float radius;
    public float surfaceGravity;
	public float mass { get; private set; }
    public Vector3 initialVelocity;
	public Vector3 velocity { get; private set; }
	Transform meshHolder;
	Rigidbody rb;

	private void Awake()
	{
		velocity = initialVelocity;
		rb = GetComponent<Rigidbody>();
		rb.mass = mass;
	}

	void OnValidate()
	{
		mass = surfaceGravity * radius * radius / Universe.gravitationalConstant;
		meshHolder = transform.GetChild(0);
		meshHolder.localScale = Vector3.one * radius;
	}

	public void UpdateVelocity(CelestialBody[] bodies, float timeStep)
	{
		foreach (var body in bodies)
		{
			if (body != this)
			{
				float distance = Vector3.Distance(transform.position, body.transform.position);
				Vector3 forceDir = (body.rb.position - rb.position).normalized;

				Vector3 acceleration = forceDir * Universe.gravitationalConstant * body.mass / distance * distance;
				velocity += acceleration * timeStep;
			}
		}
	}

	public void UpdateVelocity(Vector3 acceleration, float timeStep)
	{
		velocity += acceleration * timeStep;
	}

	public void UpdatePosition(float timeStep)
	{
		//rb.MovePosition(rb.position + velocity * timeStep);
		rb.velocity = velocity;
	}

	public Vector3 Position
	{
		get { return rb.position; }
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(transform.position, transform.position + rb.velocity * 100f);
		if (radius > 500f) return;
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(transform.position, radius * 10f);
	}
}