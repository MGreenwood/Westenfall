using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentObject : MonoBehaviour
{
    private void Start()
    {
        UIManager.instance._equipmentCanvas = GetComponent<Canvas>();
    }

    public void init()
    {

    }
}
