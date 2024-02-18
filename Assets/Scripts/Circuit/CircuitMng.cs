using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

public class CircuitMng : MonoBehaviour
{
    public GameObject[] waypoints;
    public Dictionary<CarController, int> carsPos = new Dictionary<CarController, int>();

    public float checkedWPThreshold = 4f;
    public float tooFarWPThreshold  = 15f;

    public int loops = 3;

    private void Reset()
    {
        List<GameObject> waypointList  = new List<GameObject>();
        foreach (Transform tr in transform)
        {
            waypointList.Add(tr.gameObject);
        }

        waypoints = waypointList.ToArray();
    }

    private void Update()
    {
        ManageCarsPos();
    }

    private void ManageCarsPos()
    {
        List<CarController> carsToUpdate = new List<CarController>();

        foreach (KeyValuePair<CarController, int> entry in carsPos)
        {
            Vector3 targetWP = GetResetPosition(entry.Key);
            float distanceToTargetWP = Vector3.Distance(targetWP, entry.Key.transform.position);

            // check if the car has reached the current waypoint
            if (distanceToTargetWP <= checkedWPThreshold)
            {
                carsToUpdate.Add(entry.Key);
                Debug.Log("Next Waypoint");
            }
            // check if the car is too far from the next waypoint
            else if (distanceToTargetWP >= tooFarWPThreshold)
            {
                // targetWP & nextWP direction rotation

                Vector3 nextWP = waypoints[(entry.Value + 1) % waypoints.Length].transform.position;

                Vector3 direction = nextWP - targetWP;
                Quaternion rotation = Quaternion.LookRotation(direction);

                entry.Key.ResetCar(targetWP, rotation);
            }
        }

        foreach (CarController car in carsToUpdate)
        {
            carsPos[car]++;

            if (carsPos[car] / waypoints.Length >= loops)
            {
                Debug.Log($"{car.name} WIN");

                // STOP GAME
                // if OnlineManager send message to other players
            }
        }
    }

    public Vector3 GetResetPosition(CarController car)
    {
        return waypoints[carsPos[car] % waypoints.Length].transform.position;
    }

    public Quaternion GetResetDirection(CarController car)
    {
        Vector3 nextWP = waypoints[(carsPos[car] + 1) % waypoints.Length].transform.position;

        Vector3 direction = nextWP - GetResetPosition(car);
        return Quaternion.LookRotation(direction);
    }

    private void OnDrawGizmos()
    {
        if (waypoints.Length > 0)
        {
            Vector3 prev = waypoints[0].transform.position;
            for (int i = 1; i < waypoints.Length; i++)
            {
                Vector3 next = waypoints[i].transform.position;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }

            Gizmos.DrawLine(prev, waypoints[0].transform.position);
        }
    }


}
