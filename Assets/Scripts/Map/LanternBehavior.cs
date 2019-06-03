using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LanternBehavior : MonoBehaviour
{
    [SerializeField]
    Light _light;
    [SerializeField]
    LineRenderer lr;

    [SerializeField]
    Transform pt1, pt2;

    float minTime = 0.025f;
    float maxTime = 0.35f;

    private void Start()
    {
        StartCoroutine(Flicker());
    }

    private void FixedUpdate()
    {
        // update the line renderer
        lr.SetPosition(0, pt1.position);
        lr.SetPosition(1, pt2.position);
    }

    IEnumerator Flicker()
    {
        float timeToNext = 0f;

        while(Application.isPlaying)
        {
            timeToNext = Random.Range(minTime, maxTime);
            _light.enabled = !_light.enabled;
            yield return new WaitForSeconds(timeToNext);
        }
    }
}
