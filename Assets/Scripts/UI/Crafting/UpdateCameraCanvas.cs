using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateCameraCanvas : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(WaitForCamera()); 
    }

    IEnumerator WaitForCamera()
    {
        while (Camera.main == null)
            yield return new WaitForSeconds(1f);

        Canvas c = GetComponent<Canvas>();
        c.worldCamera = Camera.main;
        c.planeDistance = 1f;
    }
}
