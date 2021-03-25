using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class growshrink : MonoBehaviour
{

    [SerializeField]
    float variationPercentage = 0.1f;
    [SerializeField]
    float lerpAmount = 0.02f;

    bool growing = true;
    Vector3 startSize;
    Vector3 grownSize;

    void Start()
    {
        startSize = transform.localScale;
        grownSize = transform.localScale * (1f + variationPercentage);
    }

    void FixedUpdate()
    {
        if (growing)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, grownSize, lerpAmount);

            if (grownSize.magnitude - transform.localScale.magnitude < 0.01f)
                growing = false;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, startSize, lerpAmount);

            if (transform.localScale.magnitude - startSize.magnitude < 0.01f)
                growing = true;
        }
    }
}
