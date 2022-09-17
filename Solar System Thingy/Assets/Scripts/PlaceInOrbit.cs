using UnityEngine;

public class PlaceInOrbit : MonoBehaviour
{
    public CelestialBody centralBody;
	public Quantity distance = new Quantity(0, Length.Unit.km, true);
	public float eccentricity;
	public Quantity semiMajorAxis;	//distance between the centre of the ellipse and the satellite
	public Quantity perihelion;		//smallest distance between two objects
	public Quantity aphelion;		//longest distance between two objects
	
	private void Awake()
	{
		/*
		float velocity = CalculateRequiredVelocity(centralBody, altitude);
		Debug.Log(velocity);
		transform.position = new Vector3(centralBody.Position.x + Length.ConvertToWorld(altitude), transform.position.y, transform.position.z);
		var body = transform.GetComponent<CelestialBody>();
		body.initialVelocity = new Vector3(0, 0, velocity);
		*/

		transform.position = new Vector3(centralBody.transform.position.x + Length.ConvertToWorld(perihelion), transform.position.y, transform.position.z);
		semiMajorAxis = CalculateSemiMajorAxis(perihelion, eccentricity);
		aphelion = CalculateAphelion(semiMajorAxis, eccentricity);
		transform.GetComponent<CelestialBody>().initialVelocity = new Vector3(0, 0, CalculateRequiredVelocity(centralBody, perihelion, semiMajorAxis));
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
}