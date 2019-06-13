using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCanvasController : MonoBehaviour
{
    Transform cam;
    Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.worldCamera = Camera.main;
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector3 lookPos = new Vector3(cam.position.x, cam.position.y / 2f, cam.position.z);
        //canvas.transform.LookAt(lookPos);

        canvas.transform.LookAt(transform.position + cam.transform.rotation * Vector3.back, cam.transform.rotation * Vector3.up);
    }
}
