using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(SocketController))]
public class OnlineManager : MonoBehaviour
{
    private SocketController socket;

    public PlayerOnlineController playerOnline;
    private string instanceName;

    private Dictionary<string, OnlineCarController> cars = 
        new Dictionary<string, OnlineCarController> ();

    public GameObject onlineCarPrefab;

    void Awake()
    {
        socket = GetComponent<SocketController>();
    }

    void Start()
    {
        instanceName = playerOnline.onlineName + playerOnline.GetHashCode();
        socket.Send("join|" + instanceName);
    }

    void Update()
    {
        
    }

    public void ParseMessages(byte[] ibuffer, int bytesReceived)
    {
        byte[] strBuffer = new byte[bytesReceived];
        Buffer.BlockCopy(ibuffer, 0, strBuffer, 0, bytesReceived);

        string str = Encoding.ASCII.GetString(ibuffer);
        Debug.Log("Data receiver: " + str);

        string[] messages = str.Split('$');
        foreach (string message in messages)
        {
            ParseMessage(message);
        }

    }

    private void ParseMessage(string message)
    {
        string[] parameters = message.Split('|');
        switch (parameters[0])
        {
            case "join":
                if (cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Player {parameters[1]} is trying to enter but is already in the scene");
                }
                else
                {
                    GameObject newPlayer = GameObject.Instantiate(onlineCarPrefab);
                    newPlayer.name = parameters[1];
                    cars.Add(parameters[1], newPlayer.GetComponent<OnlineCarController>());

                    socket.Send("join|" + instanceName);
                }
                break;

            case "updatePosition":
                if (!cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Unable to update position of {parameters[1]}, key not found");
                }
                else
                {
                    OnlineCarController car;
                    cars.TryGetValue(parameters[1], out car);

                    car.targetPosition = StringToVector3(parameters[2]);
                }
                break;

            case "updateRotation":
                if (!cars.ContainsKey(parameters[1]))
                {
                    Debug.LogWarning($"Unable to update position of {parameters[1]}, key not found");
                }
                else
                {
                    OnlineCarController car;
                    cars.TryGetValue(parameters[1], out car);

                    car.targetRotation.eulerAngles =  StringToVector3(parameters[2]);
                }
                break;
        }
    }

    public void UpdatePosition()
    {
        socket.Send("updatePosition|" + instanceName + "|" + playerOnline.transform.position);
    }

    public void UpdateRotation()
    {
        socket.Send("updateRotation|" + instanceName + "|" + playerOnline.transform.rotation.eulerAngles);
    }

    private static Vector3 StringToVector3(string strVector)
    {
        // Remove the parentheses
        if (strVector.StartsWith("(") && strVector.EndsWith(")"))
            strVector = strVector.Substring(1, strVector.Length - 2);

        // split the items
        string[] sArray = strVector.Split(',');

        return new Vector3(
            float.Parse(sArray[0], CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(sArray[1], CultureInfo.InvariantCulture.NumberFormat),
            float.Parse(sArray[2], CultureInfo.InvariantCulture.NumberFormat)
            );
    }
}
