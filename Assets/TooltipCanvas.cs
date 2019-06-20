using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipCanvas : MonoBehaviour
{
    public static TooltipCanvas instance;

    private void Awake()
    {
        instance = this;
    }
}
