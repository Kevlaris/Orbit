using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassTester : MonoBehaviour
{
    public float temp;
    void Start()
    {
        StellarClassification.Classify(temp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
