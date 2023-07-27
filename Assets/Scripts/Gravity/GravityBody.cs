using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class GravityBody : MonoBehaviour
{
    public Quantity radius;
    public float surfaceGravity;
	public float mass { get; protected set; }
    public Vector3 initialVelocity;
	public Vector3 velocity; //{ get; protected set; }
	protected Transform meshHolder;
	protected Rigidbody rb;

	public bool init = false;

	protected virtual void Awake()
	{
		rb = GetComponent<Rigidbody>();
		rb.mass = mass;
		rb.useGravity = false;
		if (init) velocity = initialVelocity;
	}

	protected virtual void Reset()
	{
		radius.Unit = Length.Unit.km;
		radius.isStatic = true;
	}

	protected virtual void OnValidate()
	{
		radius.Unit = Length.Unit.km;
		radius.isStatic = true;
		mass = Mathf.Pow(Universe.lengthScale, 3) * (surfaceGravity * Mathf.Pow(Length.Convert(radius, Length.Unit.m).amount, 2) / Universe.gravitationalConstant);
		meshHolder = transform.GetChild(0);
		meshHolder.localScale = Vector3.one * Length.ConvertToWorld(radius) * 2;
	}

	protected virtual void Start()
	{
		if (!init) Init(initialVelocity);
	}

	public void Init(Vector3 velocity)
	{
		if (!init)
		{
			initialVelocity = velocity;
			this.velocity = velocity;
			init = true;
		}
	}

	public void UpdateVelocity(GravityBody[] bodies, float timeStep)
	{
		if (!init) return;
		foreach (var body in bodies)
		{
			if (body != this)
			{
				/*
				Quantity distance = Length.ConvertFromWorld(Vector3.Distance(transform.position, body.transform.position), Length.Unit.m);
				Vector3 forceDir = (body.rb.position - rb.position).normalized;

				Vector3 acceleration = forceDir * Universe.gravitationalConstant * body.mass / Universe.lengthScale / Mathf.Pow(distance.amount, 2);
				velocity += acceleration * timeStep;
				*/

				float sqrDst = Length.ConvertFromWorld((body.transform.position - transform.position).sqrMagnitude, Length.Unit.m).amount;
				Vector3 forceDir = (body.transform.position - transform.position).normalized;
				Vector3 acceleration = body.mass * Universe.gravitationalConstant * forceDir / Universe.lengthScale / sqrDst;

				velocity += acceleration * timeStep;
			}
		}
	}

	public void UpdateVelocity(Vector3 acceleration, float timeStep)
	{
		if (!init) return;
		velocity += acceleration * timeStep;
	}

	public void UpdatePosition(float timeStep)
	{
		if (!init) return;
		//rb.MovePosition(rb.position + velocity * timeStep);
		rb.velocity = velocity;
	}
}