using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    
    public Transform player;
    Vector3 offsetDir;

    float distance = 30f, minDist = 15f, maxDist = 40f, zoomSpeed = 350f, lerpAmt = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        offsetDir = (transform.position - player.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * Time.deltaTime;
        distance = Mathf.Clamp(distance, minDist, maxDist);
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.position + offsetDir * distance, lerpAmt);
    }
}
