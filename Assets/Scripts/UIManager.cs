using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject StartMenu;
    public GameObject EndScreen;
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
        EndScreen.SetActive(false);
        UsernameField.interactable = true;
        UsernameField.text = "";
    }

    public void DisplayEndScreen(int player1CorrectFlags, int player2CorrectFlags)
    {
        EndScreen.SetActive(true);
        Text _player1Username = EndScreen.GetComponentsInChildren<Text>()[0];
        Text _player2Username = EndScreen.GetComponentsInChildren<Text>()[1];

        Text _player1Flags = EndScreen.GetComponentsInChildren<Text>()[2];
        Text _player2Flags = EndScreen.GetComponentsInChildren<Text>()[3];

        Text winner = EndScreen.GetComponentsInChildren<Text>()[4];
        _player1Username.text = GameManager.Boards[1].Username;
        _player2Username.text = GameManager.Boards[2].Username;

        _player1Flags.text = player1CorrectFlags.ToString();
        _player2Flags.text = player2CorrectFlags.ToString();

        if (player1CorrectFlags > player2CorrectFlags)
            winner.text = $"{GameManager.Boards[1].Username} has won!";
        else if (player1CorrectFlags < player2CorrectFlags)
            winner.text = $"{GameManager.Boards[2].Username} has won!";
        else
            winner.text = "Nobody won!";
    }
}
