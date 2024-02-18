using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelController : MonoBehaviour
{
    [HideInInspector]
    public WheelCollider wheelCollider;

    public bool onRoad;

    public GameObject mesh;

    private void Awake()
    {
        wheelCollider = GetComponent<WheelCollider>();
    }

    void FixedUpdate()
    {
        // mesh rotation & position
        Quaternion rotation;
        Vector3 position;
        wheelCollider.GetWorldPose(out position, out rotation);
        mesh.transform.position = position;
        mesh.transform.rotation = rotation;


        // Wheel on road
        WheelHit hit;
        if (wheelCollider.GetGroundHit(out hit))
            onRoad = hit.collider.tag == "Road";
        else
            onRoad = false;
    }


}
