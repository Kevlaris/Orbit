using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassTester : MonoBehaviour
{
    public float temperature;
    public float solarLuminosity;
    public float absoluteMagnitude;
    public bool calculateByMagnitude;
    public StellarClassification.StellarClass stellarClass;
    void Start()
    {
        ClassifyStar();
        Debug.Log(StellarClassification.ClassString(stellarClass));
    }

    void ClassifyStar()
	{
		if (calculateByMagnitude)
		{
            stellarClass = StellarClassification.ClassifyByMagnitude(temperature, absoluteMagnitude);
		}
		else
		{
            stellarClass = StellarClassification.Classify(temperature, solarLuminosity * Universe.solarLuminosity);
		}
	}
}