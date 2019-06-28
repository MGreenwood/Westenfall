using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabToNext : MonoBehaviour
{
    [SerializeField]
    InputField[] fields;

    int selectedIndex = 0;

    private void Start()
    {
        fields[selectedIndex].Select();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            selectedIndex = selectedIndex < fields.Length - 1 ? selectedIndex + 1 : 0;

            fields[selectedIndex].Select();
        }
    }
}
