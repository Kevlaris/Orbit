using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitCalculator : MonoBehaviour
{
	public float CalculateOrbitalSpeed(CelestialBody centralBody, float altitude)
	{
		return Mathf.Sqrt(Universe.gravitationalConstant * centralBody.mass / centralBody.radius + altitude);
	}

	class Orbit
	{
		double eccentricity;
		float semiMajorAxis;
		float aphelion;
		float perihelion;
		float period; // T - the time is takes for the object to complete a period
		CelestialBody centralBody;
		public Orbit(CelestialBody centralBody, float eccentricity, float semiMajorAxis)
		{
			this.centralBody = centralBody;
			this.eccentricity = eccentricity;
			this.semiMajorAxis = semiMajorAxis;
			aphelion = semiMajorAxis * (1 + eccentricity);		// A = a(1+e)
			perihelion = semiMajorAxis * (1 - eccentricity);    // P = a(1-e)
			float standardGravitationalParameter = Universe.gravitationalConstant * centralBody.mass;			// μ = G(M+m) -- m is ignored
			period = 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(semiMajorAxis, 3) / standardGravitationalParameter);   // T = 2π * sqrt(a^3/μ)
		}
		/*
		public Orbit(double eccentricity, float period)
		{
			this.eccentricity = eccentricity;
			this.period = period;
			semiMajorAxis = Mathf.Pow(period * period, 1f / 3);
			aphelion = semiMajorAxis * (1 + eccentricity);
			perihelion = semiMajorAxis * (1 - eccentricity);
		}
		*/
		public Orbit(float aphelion, float perihelion)
		{
			this.aphelion = aphelion;
			this.perihelion = perihelion;
			semiMajorAxis = (aphelion + perihelion) / 2;
			float standardGravitationalParameter = Universe.gravitationalConstant * centralBody.mass;
			period = 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(semiMajorAxis, 3) / standardGravitationalParameter);
			eccentricity = (aphelion - perihelion) / (aphelion + perihelion);	// e = (A-P) / (A+P)
		}
	}
}
