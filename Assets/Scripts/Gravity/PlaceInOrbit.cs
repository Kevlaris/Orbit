using System;
using UnityEngine;

public class PlaceInOrbit : MonoBehaviour
{
	public CelestialBody centralBody;
	CelestialBody body;
	public Quantity distance = new Quantity(0, Length.Unit.km, true);
	public float eccentricity;
	public Quantity semiMajorAxis;
	public Quantity semiMinorAxis;
	[Tooltip("Shortest distance between body and central body")] public Quantity perihelion;
	[Tooltip("Longest distance between body and central body")]  public Quantity aphelion;
	[Tooltip("The angle between the major axis of the ellipse and the position of the object")] public float theta = 0;
	[Tooltip("Inclination (the angle between the XZ axis and the plane of the orbit)")] public float phi = 0;
	[Space]
	public Vector3 initialVelocity;

	private void Reset()
	{
		body = transform.GetComponent<CelestialBody>();
	}

	private void Awake()
	{
		/*
		float velocity = CalculateRequiredVelocity(centralBody, altitude);
		Debug.Log(velocity);
		transform.position = new Vector3(centralBody.Position.x + Length.ConvertToWorld(altitude), transform.position.y, transform.position.z);
		var body = transform.GetComponent<CelestialBody>();
		body.initialVelocity = new Vector3(0, 0, velocity);
		*/

		body = transform.GetComponent<CelestialBody>();

		Recalculate();
	}

	private void Update()
	{
		distance = Length.ConvertFromWorld((transform.position - centralBody.transform.position).magnitude, Length.Unit.km);
	}

	/// <summary>
	/// Calculate required orbital velocity for stable circular orbit around central body
	/// </summary>
	/// <returns>Orbital velocity in world units</returns>
	public static float CalculateRequiredVelocity(CelestialBody centralBody, Quantity desiredAltitude)
	{
		desiredAltitude.Convert(Length.Unit.m); // convert to metres
		float velocity = Mathf.Sqrt(Universe.gravitationalConstant * (centralBody.mass / Universe.lengthScale) / (Length.Convert(centralBody.radius, Length.Unit.m) + desiredAltitude));
		return velocity;
	}
	/// <summary>
	/// Calculate required orbital velocity at distance for stable elliptic orbit around central body
	/// </summary>
	/// <param name="distance">Distance between the two objects</param>
	/// <returns>Orbital velocity in world units</returns>
	public static float CalculateRequiredVelocity(CelestialBody centralBody, Quantity distance, Quantity semiMajorAxis)
	{
		distance.Convert(Length.Unit.m); // convert to metres
		semiMajorAxis.Convert(Length.Unit.m);
		float velocity = Mathf.Sqrt(Universe.gravitationalConstant * (centralBody.mass / Universe.lengthScale) * ((2 / distance) - (1 / semiMajorAxis)));
		return velocity;
	}
	/// <summary>
	/// Calculates the initial velocity required to maintain a given elliptical orbit around a body and returns the velocity relative to the center body
	/// </summary>
	/// <param name="a">the semi-major axis of the ellipse (distance from the center of the body to one focus)</param>
	/// <param name="b">the semi-minor axis of the ellipse (distance from the center of the body to the other focus)</param>
	/// <param name="theta">the angle between the major axis of the ellipse and the position of the object in the orbit</param>
	/// <param name="phi">the angle between the XZ axis and the plane of the orbit</param>
	/// <param name="mass">the mass of the body being orbited</param>
	// source: ChatGPT
	public static Vector3 CalculateRequiredVelocity(Quantity a, Quantity b, float theta, float phi, float mass)
	{
		// Convert the semi-major and semi-minor axes to meters
		a.Convert(Length.Unit.m);
		b.Convert(Length.Unit.m);

		// Calculate the distance of the orbit from the center of the body
		float r = (a * b) / Mathf.Sqrt(Mathf.Pow(a, 2) * Mathf.Pow(Mathf.Sin(theta), 2) + Mathf.Pow(b, 2) * Mathf.Pow(Mathf.Cos(theta), 2));

		// Calculate the initial velocity required to maintain the elliptical orbit
		float v = Mathf.Sqrt(Universe.gravitationalConstant * (mass / Universe.lengthScale) / r);

		// Calculate the x, y, and z components of the velocity relative to the center body
		float z = v * Mathf.Cos(theta);	//x
		float x = v * Mathf.Sin(theta);	//y
		float y = v * Mathf.Sin(phi);	//z

		return new Vector3(x, y, z);
	}

	public static Vector3 V(Quantity _a, Quantity _p, float phi, float _mass)
	{
		Quantity a = Length.Convert(_a, Length.Unit.m);
		Quantity p = Length.Convert(_p, Length.Unit.m);

		// Calculate the required initial velocity
		float initialVelocity = Mathf.Sqrt(Universe.gravitationalConstant * (_mass / Universe.lengthScale) / p);

		// Create a Vector3 object with initial velocity in the x-axis
		Vector3 velocity = new(0f, 0f, initialVelocity);

		return velocity;
	}

public static float CalculateEccentricity(Quantity aphelion, Quantity perihelion)
	{
		if (perihelion > aphelion)
		{
			Debug.LogWarning("Entered perihelion is bigger than aphelion");
			return (perihelion - aphelion) / (perihelion + aphelion).amount;
		}

		return (aphelion - perihelion) / (aphelion + perihelion).amount;
	}

	public static Quantity CalculatePerihelion(Quantity semiMajorAxis, float eccentricity)
	{
		if (eccentricity == 0)
		{
			return semiMajorAxis;
		}
		else
		{
			return new Quantity(semiMajorAxis * (1 - eccentricity), semiMajorAxis.Unit);
		}
	}
	public static Quantity CalculateAphelion(Quantity semiMajorAxis, float eccentricity)
	{
		if (eccentricity < 0)
		{
			Debug.LogError("Eccentricity of orbit must be greater than zero");
		}
		if (eccentricity == 0)
		{
			return semiMajorAxis;
		}
		else if (eccentricity >= 1)	//parabola or hyperbola
		{
			return new Quantity(float.PositiveInfinity, semiMajorAxis.Unit);
		}
		else
		{
			return new Quantity(semiMajorAxis * (1 + eccentricity), semiMajorAxis.Unit);
		}
	}

	public static Quantity CalculateSemiMajorAxis(Quantity perihelion, float eccentricity)
	{
		return new Quantity(perihelion / (1 - eccentricity), perihelion.Unit);
	}
	public static Quantity CalculateSemiMajorAxis(float eccentricity, Quantity aphelion)
	{
		return new Quantity(aphelion / (1 + eccentricity), aphelion.Unit);
	}

	public static Quantity CalculateSemiMinorAxis(Quantity semiMajorAxis, float eccentricity)
	{
		Quantity majorAxis = new Quantity(semiMajorAxis*2, semiMajorAxis.Unit);
		Quantity minorAxis = new Quantity(majorAxis * Mathf.Sqrt(1 - Mathf.Pow(eccentricity, 2)), majorAxis.Unit);
		return new Quantity(minorAxis / 2, minorAxis.Unit);
	}

	public void Recalculate()
	{
		if (body == null) body = transform.GetComponent<CelestialBody>();

		if (perihelion > aphelion)
		{
			(perihelion, aphelion) = (aphelion, perihelion);	//swap if perihelion is bigger
		}

		if (perihelion != 0 && aphelion != 0) eccentricity = CalculateEccentricity(aphelion, perihelion);

		semiMajorAxis = CalculateSemiMajorAxis(perihelion, eccentricity);
		semiMinorAxis = CalculateSemiMinorAxis(semiMajorAxis, eccentricity);

		//initialVelocity = CalculateRequiredVelocity(semiMajorAxis, semiMinorAxis, theta, phi, centralBody.mass);
		initialVelocity = V(semiMajorAxis, perihelion, phi, centralBody.mass);
	}

	public void SetParameters()
	{
		Recalculate();
		Quantity x = new(perihelion.amount * Mathf.Cos(Mathf.Deg2Rad * phi), perihelion.Unit);
		Quantity y = new(perihelion.amount * Mathf.Sin(Mathf.Deg2Rad * phi), perihelion.Unit);
		Debug.Log($"x={x}; y={y}; sin={Mathf.Sin(Mathf.Deg2Rad * phi)}");
		transform.position = new Vector3(centralBody.transform.position.x + Length.ConvertToWorld(x), centralBody.transform.position.y + Length.ConvertToWorld(y), centralBody.transform.position.z);
		body.initialVelocity = initialVelocity;
	}
}