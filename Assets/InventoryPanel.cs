using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanel : MonoBehaviour, IDroppableArea
{
    [SerializeField]
    Inventory _inventory;

    public Inventory GetInventoryObject()
    {
        return _inventory;
    }
}
