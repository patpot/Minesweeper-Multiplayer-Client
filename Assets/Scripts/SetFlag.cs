using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SetFlag : MonoBehaviour, IPointerClickHandler
{
    public int X;
    public int Y;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ClientSend.SendFlag(X, Y);
        }
    }
}
