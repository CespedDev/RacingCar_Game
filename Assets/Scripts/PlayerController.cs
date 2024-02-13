using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private CarController car;

    private void Awake()
    {
        car = GetComponent<CarController>();
    }

    private void Update()
    {
        
    }


}
