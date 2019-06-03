using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatTextCanvas : MonoBehaviour
{
    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        GetComponent<Canvas>().worldCamera = Camera.main;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);
    }
}
