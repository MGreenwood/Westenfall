using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class Equipment: MonoBehaviour
{
    [SerializeField]
    InventoryEquipmentSlot[] _equipmentSlots;

    Transform _slot;

    public InventoryEquipmentSlot GetSlotContents(int index)
    {
        if (index >= 0 && index < _equipmentSlots.Length)
            return _equipmentSlots[index];
        else return null;
    }

    public Attributes GetEquipmentAttributes()
    {
        Attributes attr = new Attributes();

        foreach(InventoryEquipmentSlot slot in _equipmentSlots)
        {
            
        }

        return attr;
    }

    public void Equip(InventoryItemObject item, int slot)
    {
        _equipmentSlots[slot].SetItem(item);
    }
}
