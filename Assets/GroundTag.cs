using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GroundTag : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerDown(PointerEventData pointerEventData)
    {
        if(GroundItemCanvas.instance.ClickedGroundTag(transform.parent))
            UIManager.instance.RegisterOverUI(false);
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        UIManager.instance.RegisterOverUI(true);
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        UIManager.instance.RegisterOverUI(false);
    }
}
