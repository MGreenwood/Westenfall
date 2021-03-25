using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody rb;
    Vector2 input;

    float _speed = 0f;
    float _startingSpeed = 1000f;
    float _MAX_SPEED = 0f;
    float _startingMaxSpeed = 10f;
    float _slow = 0.8f;

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
        GetComponent<PlayerAbilities>().D_ControllingMovement += MovementPaused;
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForEndOfFrame();
        playerAttributes = GetComponent<Player>().GetAttributes();
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
        print("movement paused on object -- " + name);
    }

    private void FixedUpdate()
    {
        if (currentKnock != null || playerAttributes == null)
            return;

        _speed = _startingSpeed + playerAttributes.GetArmorStat(Armor.Stats.MoveSpeed)._value;
        _MAX_SPEED = _startingMaxSpeed + (0.001f * _speed);

        if (!_isPaused)
        {
            // input & movement
            input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            rb.AddForce(new Vector3(input.x, 0, input.y) * _speed, ForceMode.Force);
        }
        //slow
        if(currentKnock == null)
            rb.velocity = new Vector3(rb.velocity.x * _slow, rb.velocity.y, rb.velocity.z * _slow);


        // speed limit
        if (rb.velocity.magnitude > _MAX_SPEED + playerAttributes.GetArmorStat(Armor.Stats.MoveSpeed)._value && !delayMaxSpeed)
            rb.velocity = rb.velocity.normalized * (_MAX_SPEED + playerAttributes.GetArmorStat(Armor.Stats.MoveSpeed)._value);

        if (rb.velocity.magnitude > 1f)
        {
            anim.SetBool("Running", true);

            Vector3 mouse = mPos();
            mouse.y = transform.position.y;
            float percent = rb.velocity.magnitude / _MAX_SPEED;

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

        rb.AddForce(LookupDictionary._knockbackForce * power * (transform.position - source).normalized, ForceMode.Impulse);
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
