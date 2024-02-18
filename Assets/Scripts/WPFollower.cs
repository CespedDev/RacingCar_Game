using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WPFollower : MonoBehaviour
{
    public CircuitMng circuit;
    public int currentWPIndex = 0;
    public Vector3 currentWPPosition;

    public float speed;
    public float rotationSpeed;

    public float distanceToWPThreshold = 1f;

    // Start is called before the first frame update
    void Start()
    {
        currentWPPosition = circuit.waypoints[currentWPIndex].transform.position;   

    }

    // Update is called once per frame
    void Update()
    {
        float distanceToWP = Vector3.Distance(transform.position, currentWPPosition);

        Vector3 targetDirection = currentWPPosition - transform.position;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(targetDirection), rotationSpeed *
            Time.deltaTime);

        transform.Translate(0f, 0f, speed * Time.deltaTime);

        if (distanceToWP <= distanceToWPThreshold)
        {
            currentWPIndex = (currentWPIndex + 1) % circuit.waypoints.Length;
            currentWPPosition = circuit.waypoints[currentWPIndex].transform.position;
        }
    }
}
