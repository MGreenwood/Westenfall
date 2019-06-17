using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookupDictionary : MonoBehaviour
{
    public static LookupDictionary instance;

    private void Awake()
    {
        instance = this;
    }

    [ColorUsageAttribute(true, true)]
    public Color[] colors;
}
