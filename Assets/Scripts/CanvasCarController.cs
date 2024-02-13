using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CanvasCarController : MonoBehaviour
{
    public TMP_Text      speedometer;
    public CarController carController;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        speedometer.text = Mathf.Round(carController.actualSpeed) + " Km/H";
    }
}
