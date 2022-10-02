using UnityEngine;

public static class Universe
{
    public const float gravitationalConstant = 1;
    public const float physicsTimeStep = 0.01f;
    public const float lengthScale = 0.0001f;
    public static float solarLuminosity = 3.8f * Mathf.Pow(10, 26);
    public static float zeroPointLuminosity = 3f * Mathf.Pow(10, 28);
    /// <summary>
    /// Equatorial radius of the Sun. (in km)
    /// </summary>
    public static Quantity solarRadius = new Quantity(695700, Length.Unit.km, true);
    public static GameObject solarParticleStandard = Resources.Load<GameObject>("SolarParticleStandard");
}