using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CarController : MonoBehaviour
{
    [HideInInspector]
    public WheelController[] frontWheels = new WheelController[2];
    [HideInInspector]
    public WheelController[] rearWheels  = new WheelController[2];
    [HideInInspector]
    public WheelController[] wheels      = new WheelController[4];
    [SerializeField]
    private ParticleSystem[] skidSmokes  = new ParticleSystem[2];

    public  Rigidbody rigidbody;
    private Transform centerOfMass;

    public float torque = 1000f;
    public float brakeTorque = 2000f;
    public float maxSteerAngle = 30f;

    public float maxSpeed = 100f;

    public float actualSpeed
    {
        // km/h
        get { return rigidbody.velocity.magnitude * 3.6f; }
    }

    public enum CarType
    {
        FWD,
        RWD,
        AWD
    }

    public CarType carType = CarType.AWD;

    public float antiRoll = 5000f;

    private float[] skidValues = new float[4];
    public AudioSource skidSound;
    public float skidThreshold = 0.1f;

    public AudioSource engineSound;
    public AnimationCurve engineSoundCurve;
    public float engineSoundMinPitch = 0.2f;
    public float engineSoundMaxPitch = 2.2f;

    public CircuitMng circuit;

    public float timeOutRoad = 2f;
    private float timeOutRoadAux;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();

        centerOfMass = transform.Find("CenterOfMass");
        if (centerOfMass)
            rigidbody.centerOfMass = centerOfMass.localPosition;

        // get the wheels references
        frontWheels[0] = transform.Find("FrontLeftWheel").GetComponent<WheelController>();
        frontWheels[1] = transform.Find("FrontRightWheel").GetComponent<WheelController>();
        rearWheels[0]  = transform.Find("RearLeftWheel").GetComponent<WheelController>();
        rearWheels[1]  = transform.Find("RearRightWheel").GetComponent<WheelController>();

        wheels[0] = frontWheels[0];
        wheels[1] = frontWheels[1];
        wheels[2] = rearWheels[0];
        wheels[3] = rearWheels[1];

        timeOutRoadAux = timeOutRoad;
    }

    void Start()
    {
        circuit.carsPos.Add(this, 0);
    }

    void FixedUpdate()
    {
        // AntiRollBar
        GroundWheels(frontWheels[0].wheelCollider, frontWheels[1].wheelCollider);
        GroundWheels(rearWheels[0].wheelCollider, rearWheels[1].wheelCollider);

        CheckSkid();

        CalculateEngineSound();

        CheckOutOfTrack();

        CheckReversed();
    }

    public void ApplyTorque(float torqueInput)
    {
        switch (carType)
        {
            case CarType.FWD:
                foreach (WheelController wheel in frontWheels)
                {
                    wheel.wheelCollider.motorTorque = torqueInput * torque * ((maxSpeed - actualSpeed) / maxSpeed);
                }
                break;
            case CarType.RWD:
                foreach (WheelController wheel in rearWheels)
                {
                    wheel.wheelCollider.motorTorque = torqueInput * torque * ((maxSpeed - actualSpeed) / maxSpeed);
                }
                break;
            case CarType.AWD:
                foreach (WheelController wheel in wheels)
                {
                    wheel.wheelCollider.motorTorque = torqueInput * torque * ((maxSpeed - actualSpeed) / maxSpeed);
                }
                break;
        }
    }

    public void ApplyBrake(float brakeInput)
    {
        float brake = brakeTorque * brakeInput;

        foreach (WheelController wheel in wheels)
        {
            wheel.wheelCollider.brakeTorque = brake;
        }
    }

    public void ApplySteering(float steerInput)
    {
        float steer = steerInput * maxSteerAngle;

        foreach (WheelController wheel in frontWheels)
        {
            wheel.wheelCollider.steerAngle = steer;
        }
    }

    public void ResetCar(Vector3 pos, Quaternion rot)
    {
        rigidbody.velocity = Vector3.zero;
        transform.position = pos;
        transform.rotation = rot;
    }

    private void CheckOutOfTrack ()
    {
        foreach (WheelController wheel in wheels)
        {
            if (wheel.onRoad)
            {
                timeOutRoadAux = timeOutRoad;
                return;
            }
        }

        timeOutRoadAux -= Time.deltaTime;

        if (timeOutRoadAux <= 0)
        {
            ResetCar(circuit.GetResetPosition(this), circuit.GetResetDirection(this));
            timeOutRoadAux = timeOutRoad;
        }

    }

    private void CheckReversed()
    {
        if (transform.rotation.eulerAngles.x > 140 && transform.rotation.eulerAngles.x < 220 ||
            transform.rotation.eulerAngles.z > 140 && transform.rotation.eulerAngles.z < 220)
        {
            ResetCar(circuit.GetResetPosition(this), circuit.GetResetDirection(this));
        }
    }

    private void GroundWheels(WheelCollider leftWheel, WheelCollider rightWheel)
    {
        WheelHit hit;
        float leftTravel = 1f, rightTravel = 1f;

        // calculate the proportions of how grounded each wheel is
        bool leftGrounded = leftWheel.GetGroundHit(out hit);
        if (leftGrounded)
            leftTravel = (-leftWheel.transform.InverseTransformPoint(hit.point).y - leftWheel.radius) / leftWheel.suspensionDistance;

        bool rightGrounded = rightWheel.GetGroundHit(out hit);
        if (rightGrounded)
            rightTravel = (-rightWheel.transform.InverseTransformPoint(hit.point).y - rightWheel.radius) / rightWheel.suspensionDistance;

        float antiRollForce = (leftTravel - rightTravel) * antiRoll;

        if (leftGrounded)
            rigidbody.AddForceAtPosition(leftWheel.transform.up * -antiRollForce, leftWheel.transform.position);

        if (rightGrounded)
            rigidbody.AddForceAtPosition(rightWheel.transform.up * -antiRollForce, rightWheel.transform.position);
    }

    private void CheckSkid()
    {
        int wheelsSkidding = 0; // number of wheels skidding
        WheelHit wheelHit;

        for (int i = 0; i < wheels.Length; i++)
        {
            wheels[i].wheelCollider.GetGroundHit(out wheelHit);

            float forwardSlip = Mathf.Abs(wheelHit.forwardSlip);
            float sidewaysSlip = Mathf.Abs(wheelHit.sidewaysSlip);

            if (forwardSlip >= skidThreshold ||
                sidewaysSlip >= skidThreshold)
            {
                wheelsSkidding++;
                skidValues[i] = forwardSlip + sidewaysSlip;
            }
            else
                skidValues[i] = 0f;
        }

        // skidding sound
        if (wheelsSkidding == 0 && skidSound.isPlaying)
        {
            skidSound.Stop();

            foreach (ParticleSystem particle in skidSmokes)
            {
                particle.Stop();
            }
        }
        else if (wheelsSkidding > 0)
        {
            // update the drifting volume
            skidSound.volume = (float)wheelsSkidding / wheels.Length;

            skidSound.panStereo = -skidValues[0] + skidValues[1] - skidValues[2] + skidValues[3];

            if (!skidSound.isPlaying)
            {
                skidSound.Play();

                foreach (ParticleSystem particle in skidSmokes)
                {
                    particle.Play();
                }
            }
                
        }
    }

    private void CalculateEngineSound()
    {
        float speedProp = actualSpeed / maxSpeed;
        engineSound.pitch = Mathf.Lerp(engineSoundMinPitch, engineSoundMaxPitch,
            engineSoundCurve.Evaluate(speedProp));
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < skidValues.Length; i++)
        {
            if (wheels[i])
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(wheels[i].transform.position, skidValues[i]);
            }
        }
    }

}
