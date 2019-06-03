using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public bool _isOverUI = false;
    public bool _isHoldingItem = false;
    Item _heldItem;

    public Canvas _equipmentCanvas;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void RegisterOverUI(bool isOverUI)
    {
        _isOverUI = isOverUI;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab) && _equipmentCanvas != null)
        {
            _equipmentCanvas.enabled = !_equipmentCanvas.enabled;
        }
    }

    public void PickupItem(Item held)
    {
        _isHoldingItem = true;
        _heldItem = held;
    }

    public void DropItem()
    {
        _isHoldingItem = false;
        _heldItem = null;
    }
}
