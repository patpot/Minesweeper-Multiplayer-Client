using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject StartMenu;
    public InputField UsernameField;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    public void ConnectToServer()
    {
        if (UsernameField.text.Length > 12)
        {
            MessageManager.Instance.DisplayAMessage(3f, "Usernames must be 12 characters or less!");
            return;
        }
        StartMenu.SetActive(false);
        UsernameField.interactable = false;
        Client.Instance.ConnectToServer();
    }

    public void ReturnToMenu()
    {
        StartMenu.SetActive(true);
        UsernameField.interactable = true;
        UsernameField.text = "";
    }
}
