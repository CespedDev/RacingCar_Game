using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerOnlineController : MonoBehaviour
{
    public string onlineName;

    public float timeToUpdatePosition     = 0.5f;
    private float timeToUpdatePositionAux = 0.5f;
    public float timeToUpdateRotation     = 0.5f;
    private float timeToUpdateRotationAux = 0.5f;

    public OnlineManager onlineManager;

    private void Start()
    {
        timeToUpdatePositionAux = timeToUpdatePosition;
        timeToUpdateRotationAux = timeToUpdateRotation;
    }

    void LateUpdate()
    {
        timeToUpdatePositionAux -= Time.deltaTime;
        timeToUpdateRotationAux -= Time.deltaTime;

        if(timeToUpdatePositionAux <= 0)
        {
            onlineManager.UpdatePosition();
            timeToUpdatePositionAux = timeToUpdatePosition;
        }

        if (timeToUpdateRotationAux <= 0)
        {
            onlineManager.UpdateRotation();
            timeToUpdateRotationAux = timeToUpdateRotation;
        }
    }
}
