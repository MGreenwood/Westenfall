using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollRectStartAtTop : MonoBehaviour
{
    ScrollRect sr;

    void Start()
    {
        sr = GetComponent<ScrollRect>();
        StartCoroutine(ScrollToTopAfterDelay());
    }

    IEnumerator ScrollToTopAfterDelay()
    {
        yield return new WaitForSeconds(2);

        sr.normalizedPosition = new Vector2(0, 1);
    }
}
