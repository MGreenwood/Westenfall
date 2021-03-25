using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTextDestroy : MonoBehaviour {

    Camera cam;
    float sideScroll = 0f;
    const float SCROLL_AMOUNT = 0.5f;
    const float TIME_TO_DESTROY = 1.5f;

    Transform parentWorld;
    Vector3 offset;

    RectTransform rt;

	void Start ()
    {
        Destroy(this.gameObject, TIME_TO_DESTROY);
        cam = Camera.main;
        rt = GetComponent<RectTransform>();

        sideScroll = Random.Range(-SCROLL_AMOUNT, SCROLL_AMOUNT);

        StartCoroutine("_RandomMove");
    }

    public void SetParentWorldTransform(Transform t)
    {
        parentWorld = t;
    }

    IEnumerator _RandomMove()
    {
        while(Application.isPlaying)
        {
            rt.localPosition += new Vector3(sideScroll, 0);

            yield return new WaitForSeconds(.01f);
        }
    }
}
