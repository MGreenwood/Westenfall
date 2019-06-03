using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Transform playerCam;

    float lerpAmt = 0.2f;

    float zOffset;
    Vector3 offset;
    float speed = 0.2f;


    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, playerCam.position, lerpAmt);

        Vector3 direction = playerCam.GetComponent<CameraFollowObject>().player.position - transform.position;
        Quaternion toRotation = Quaternion.LookRotation(direction, transform.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed);
    }
}
