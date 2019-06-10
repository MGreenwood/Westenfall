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

    [SerializeField]
    float _rotationSpeed = 0.2f;

    [SerializeField]
    Animator anim;

    delegate Vector3 mousePos(); // keep a reference to the mouse position method
    mousePos mPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mPos = MouseManager.instance.GetMousePosition;
    }

    void Update()
    {
        // face the mouse position INSTANT LOOK
        Vector3 p = mPos();
        //transform.LookAt(new Vector3(p.x, transform.position.y, p.z));

        Vector3 dir = (p - transform.position).normalized;
        dir.y = 0; // keep it horizontal

        if (dir != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, _rotationSpeed);
        }
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

        if (rb.velocity.magnitude > 1f)
        {
            anim.SetBool("Running", true);

            Vector3 mouse = mPos();
            mouse.y = transform.position.y;
            float percent = rb.velocity.magnitude / MAX_SPEED;

            // get angle difference between mouseDir and facingDir
            float diff = Vector3.Angle(rb.velocity.normalized, (mouse - transform.position).normalized);
            if (diff > 90f) // running backwards
            {
                anim.SetFloat("Speed", -percent * 1.3f); // feet move faster when running backwards
            }
            else
            {
                anim.SetFloat("Speed", percent);
            }
        }
        else
        {
            anim.SetBool("Running", false);
            anim.SetFloat("Speed", 1f);
        }
    }
}
