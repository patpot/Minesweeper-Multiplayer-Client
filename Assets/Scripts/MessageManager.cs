using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;
    public GameObject Message;

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

    public void DisplayAMessage(float time, string message) => StartCoroutine(DisplayMessage(time, message));

    IEnumerator DisplayMessage(float time, string message)
    {
        Message.SetActive(true);
        Message.GetComponentInChildren<Text>().text = message;
        yield return new WaitForSeconds(time);

        Message.SetActive(false);
    }
}
