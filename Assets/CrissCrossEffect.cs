using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrissCrossEffect : MonoBehaviour
{
    // tf1 and tf2 must be placed in same position and rotation as parent object

    [SerializeField]
    Transform tf1, tf2; // the two transforms to cross
    [SerializeField]
    float range = 80f, spread = 2f, speed = 0.001f;

    Vector3 endpoint;


    void Start()
    {
        tf1.localPosition += new Vector3(-spread, 0, 0);
        tf2.localPosition += new Vector3(spread, 0, 0);

        Vector3 tf1Forward = tf1.position + tf1.forward * range;
        Vector3 tf2Forward = tf2.position + tf2.forward * range;

        tf1.LookAt(tf2Forward, Vector3.up);
        tf2.LookAt(tf1Forward, Vector3.up);

        StartCoroutine(MoveForward());
    }

    IEnumerator MoveForward()
    {
        float distTraveled = 0f;

        while (distTraveled < range)
        {
            tf1.position += tf1.forward * speed;
            tf2.position += tf2.forward * speed;
            distTraveled += speed;

            yield return new WaitForEndOfFrame();
        }



        Destroy(gameObject);
    }
}
