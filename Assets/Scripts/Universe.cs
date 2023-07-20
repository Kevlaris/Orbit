using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Universe
{
	public const float gravitationalConstant = 1;
	public const float physicsTimeStep = .01f;
	public const float lengthScale = 0.000001f;
	public static float solarLuminosity = 3.8f * Mathf.Pow(10, 26);
	public static float zeroPointLuminosity = 3f * Mathf.Pow(10, 28);
	/// <summary>
	/// Equatorial radius of the Sun. (in km)
	/// </summary>
	public static Quantity solarRadius = new Quantity(695700, Length.Unit.km, true);

	public static GameObject solarParticleStandard = Resources.Load<GameObject>("Stars/Particles/SolarParticleStandard");
	public static SpectralClassification harvard = Resources.Load<SpectralClassification>("Stars/Classifications/Harvard");
	public static LuminosityClassification yerkes = Resources.Load<LuminosityClassification>("Stars/Classifications/Yerkes");

	static List<int> usedIDs = new List<int>();
	static Dictionary<int, CelestialBody> celestialBodies = new();
	public static int RequestID(CelestialBody celestialBody)
	{
		int id;
		if (usedIDs.Count == 0)
		{
			id = 0;
		}
		else
		{
			id = usedIDs.Last() + 1;
		}

		celestialBodies.Add(id, celestialBody);
		usedIDs.Add(id);
		return id;
	}
	public static CelestialBody GetCelestialBody(int id)
	{
		return celestialBodies[id];
	}
}