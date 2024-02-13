using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Transform target;
    public Vector3 offset;
    public float followSpeed = 5f;
    public float rotationSpeed = 5f;

    void LateUpdate()
    {
        // follow update
        Vector3 targetPosition = target.position +
                                 target.forward * offset.z +
                                 target.right * offset.x +
                                 ((target.up.y >= 1f) ? target.up * offset.y : Vector3.up * offset.y);
        transform.position = Vector3.Slerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // look update
        Vector3 lookDirection = target.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDirection, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
    }
}
