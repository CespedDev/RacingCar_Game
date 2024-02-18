using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineCarController : MonoBehaviour
{
    public float updatePositionSpeed = 1.0f;
    public float updateRotationSpeed = 1.0f;

    [HideInInspector]
    public Vector3 targetPosition;
    [HideInInspector]
    public Quaternion targetRotation;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position,
            targetPosition, updatePositionSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation,
            targetRotation, updateRotationSpeed * Time.deltaTime);
    }
}
