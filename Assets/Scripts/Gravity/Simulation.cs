using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : MonoBehaviour
{
    GravityBody[] bodies;
    static Simulation instance;

    void Awake()
    {

        bodies = FindObjectsOfType<GravityBody>();
        Time.fixedDeltaTime = Universe.physicsTimeStep;
        Debug.Log("Setting fixedDeltaTime to: " + Universe.physicsTimeStep);
    }

    void FixedUpdate()
    {
        for (int i = 0; i < bodies.Length; i++)
        {
            //Vector3 acceleration = CalculateAcceleration(bodies[i].Position, bodies[i]);
            //bodies[i].UpdateVelocity(acceleration, Universe.physicsTimeStep);
            bodies[i].UpdateVelocity (bodies, Universe.physicsTimeStep);
        }

        for (int i = 0; i < bodies.Length; i++)
        {
            bodies[i].UpdatePosition(Universe.physicsTimeStep);
        }

    }

    public static Vector3 CalculateAcceleration(Vector3 point, GravityBody ignoreBody = null)
    {
        Vector3 acceleration = Vector3.zero;
        foreach (var body in Instance.bodies)
        {
            if (body != ignoreBody)
            {
                float sqrDst = Length.ConvertFromWorld((body.transform.position - point).sqrMagnitude, Length.Unit.m).amount;
                Vector3 forceDir = (body.transform.position - point).normalized;
                acceleration += forceDir * Universe.gravitationalConstant * body.mass / Universe.lengthScale / sqrDst;
            }
        }

        return acceleration;
    }

    public static GravityBody[] Bodies
    {
        get
        {
            return Instance.bodies;
        }
    }

    static Simulation Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Simulation>();
            }
            return instance;
        }
    }
}