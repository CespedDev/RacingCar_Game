using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalServerInfo", menuName = "ScriptableObjects/LocalServerInfo", order = 1)]
public class LocalServerInfoSO : ScriptableObject
{
    public string IP   = "127.0.0.1";
    public int    Port = 666;
}
