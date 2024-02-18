using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    private CarController car;
    public CircuitMng circuit;

    public float steeringSensivity = 0.01f;
    public float acelerationSensivity = 1f;
    public float brakeSensivity = 0.1f;

    private Vector3 targetWP;
    private int targetWPIndex;

    public float distanceToWPThreshold = 4f;

    public float cornerDegrees = 90f;

    void Awake()
    {
        car = GetComponent<CarController>();
    }

    void Start()
    {
        targetWPIndex = 0;
        targetWP = circuit.waypoints[targetWPIndex].transform.position;
    }

    void FixedUpdate()
    {
        Vector3 localTarget = transform.InverseTransformPoint(targetWP);
        float targetAngle = Mathf.Atan2(localTarget.x, localTarget.z) *
            Mathf.Rad2Deg;

        float distanceToTargetWP = Vector3.Distance(targetWP, 
            transform.position);
        float speedFactor = car.actualSpeed / car.maxSpeed;
        float corner = Mathf.Clamp(targetAngle, -cornerDegrees, cornerDegrees);
        float cornerFactor = corner / cornerDegrees;

        Debug.DrawLine(transform.position, targetWP, Color.magenta);

        float torque = 1f;
        float brake = 0f;
        float steer = Mathf.Clamp(targetAngle * steeringSensivity, -1f, 1f) *
            Mathf.Sign(car.actualSpeed);

        if (speedFactor >= .08f && cornerFactor >= .1f)
        {
            brake = Mathf.Lerp(0f, .5f + (speedFactor * brakeSensivity), cornerFactor);
        
        }

        if (speedFactor >= 0.12f && cornerFactor >= 0.4f)
        {
            torque = Mathf.Lerp(0f, acelerationSensivity, 1f - cornerFactor);
        }

        car.ApplyTorque(torque);
        car.ApplyBrake(brake);
        car.ApplySteering(steer);

        // check if the car has reached the current waypoint
        if(distanceToTargetWP <= distanceToWPThreshold)
        {
            targetWPIndex = (targetWPIndex + 1) % circuit.waypoints.Length;
            targetWP = circuit.waypoints[targetWPIndex].transform.position;

        }
    }
}
