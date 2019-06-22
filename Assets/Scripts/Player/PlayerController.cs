using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector2 input;

    float speed = 0f;
    float startingSpeed = 200f;
    float MAX_SPEED = 10f;
    float startingMAxSpeed = 10f;
    float slow = 0.9f;

    float _knockbackForce = 200f;
    public const float delayTime = 0.3f;
    bool delayMaxSpeed = false;
    Coroutine currentKnock;

    [SerializeField]
    float _rotationSpeed = 0.2f;

    bool _isPaused = false;

    [SerializeField]
    Animator anim;

    Attributes playerAttributes;

    delegate Vector3 mousePos(); // keep a reference to the mouse position method
    mousePos mPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mPos = MouseManager.instance.GetMousePosition;
        playerAttributes = GetComponent<Player>().GetAttributes();
        GetComponent<PlayerAbilities>().D_ControllingMovement += MovementPaused;
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

    public void MovementPaused(bool isPaused)
    {
        _isPaused = isPaused;
    }

    private void FixedUpdate()
    {
        if (_isPaused)
            return;

        speed = startingSpeed + playerAttributes.GetArmorStat(Armor.Stats.MoveSpeed)._value;
        MAX_SPEED = startingMAxSpeed + (0.1f * speed);

        // input & movement
        input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        rb.AddForce(new Vector3(input.x, 0, input.y) * speed, ForceMode.Force);

        //slow
        rb.velocity = new Vector3(rb.velocity.x * slow, rb.velocity.y, rb.velocity.z * slow);


        // speed limit
        if (rb.velocity.magnitude > MAX_SPEED + playerAttributes.GetArmorStat(Armor.Stats.MoveSpeed)._value && !delayMaxSpeed)
            rb.velocity = rb.velocity.normalized * (MAX_SPEED + playerAttributes.GetArmorStat(Armor.Stats.MoveSpeed)._value);

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

    internal void Knockback(Vector3 source, int power)
    {
        if(currentKnock != null)
            StopCoroutine(currentKnock);

        rb.AddForce((_knockbackForce * power) * (transform.position - source).normalized, ForceMode.Impulse);
        delayMaxSpeed = true;
        currentKnock = StartCoroutine(KnockbackEnumerator());
    }

    IEnumerator KnockbackEnumerator()
    {
        yield return new WaitForSeconds(delayTime);
        delayMaxSpeed = false;
        currentKnock = null;
    }
}
