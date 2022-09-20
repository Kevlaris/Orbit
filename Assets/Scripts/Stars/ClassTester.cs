using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassTester : MonoBehaviour
{
    public float temperature;
    public float bolometricLuminosity;
    void Start()
    {
        StellarClassification.Classify(temperature, bolometricLuminosity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
