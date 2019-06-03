using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector2 input;

    float speed = 200f;
    float MAX_SPEED = 10f;
    float slow = 0.9f;

    delegate Vector3 mousePos(); // keep a reference to the mouse position method
    mousePos mPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mPos = MouseManager.instance.GetMousePosition;
    }

    void Update()
    {
        // face the mouse position
        Vector3 p = mPos();
        transform.LookAt(new Vector3(p.x, transform.position.y, p.z));
    }

    private void FixedUpdate()
    {
        // input & movement
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.AddForce(new Vector3(input.x, 0, input.y) * speed, ForceMode.Force);

        //slow
        rb.velocity = new Vector3(rb.velocity.x * slow, rb.velocity.y, rb.velocity.z * slow);

        // speed limit
        if (rb.velocity.magnitude > MAX_SPEED)
            rb.velocity = rb.velocity.normalized * MAX_SPEED;
    }
}
