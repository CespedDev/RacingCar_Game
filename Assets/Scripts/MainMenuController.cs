using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private LocalServerInfoSO localServer;

    [SerializeField]
    private GameObject gameModeSelector;

    [SerializeField]
    private GameObject serverSelector;

    [SerializeField]
    private TMP_InputField serverField;

    [SerializeField]
    private TMP_InputField portField;


    void Start()
    {
        gameModeSelector.SetActive(true);
        serverSelector  .SetActive(false);
    }

    public void SingleSelected()
    {
        SceneManager.LoadScene("AICircuit");
    }

    public void OnlineSelected()
    {
        gameModeSelector.SetActive(false);
        serverSelector  .SetActive(true);
    }

    public void StartOnline()
    {
        localServer.IP   =   serverField.text;
        localServer.Port =   Int32.Parse(portField.text);
        SceneManager.LoadScene("OnlineCircuit");
    }
}
