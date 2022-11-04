using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerCam;

    float lerpAmt = 0.99f;

    float zOffset;
    Vector3 offset;
    float speed = 0.45f;


    private void FixedUpdate()
    {
        transform.position = playerCam.position;
        //transform.position = Vector3.Lerp(transform.position, playerCam.position, lerpAmt);

        Vector3 direction = playerCam.GetComponent<CameraFollowObject>().player.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed);
    }
}
