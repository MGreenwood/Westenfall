using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void IPointerEnterHandler.OnPointerEnter(PointerEventData data)
    {
        UIManager.instance.RegisterOverUI(true);
        Debug.Log("Entered UI");
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.RegisterOverUI(false);
        Debug.Log("Exited UI");
    }
}
