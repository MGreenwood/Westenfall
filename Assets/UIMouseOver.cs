using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    void IPointerEnterHandler.OnPointerEnter(PointerEventData data)
    {
        UIManager.instance.RegisterOverUI(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        UIManager.instance.RegisterOverUI(false);
    }
}
