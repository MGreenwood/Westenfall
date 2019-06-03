using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public static MouseManager instance; // static instance

    [SerializeField] LayerMask ground;
    private bool _waiting = true;

    Camera cam;
    Vector3 mousepos;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        StartCoroutine(WaitForCamera());

        mousepos = Vector3.zero;
    }

    private void Update()
    {
        if (_waiting)
            return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, ground))
        {
            mousepos = hit.point;
        }
    }

    public Vector3 GetMousePosition()
    {
        return mousepos;
    }

    IEnumerator WaitForCamera()
    {
        while(_waiting)
        {
            if (Camera.main != null)
            {
                cam = Camera.main;
                _waiting = false;
            }

            yield return new WaitForSeconds(0.4f);
        }

    }
}
