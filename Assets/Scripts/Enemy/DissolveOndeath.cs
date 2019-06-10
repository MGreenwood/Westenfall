using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolveOndeath : MonoBehaviour
{
    [SerializeField]
    Shader dissolveShader;

    public float reduction = 0.02f;

    private void Start()
    {
        GetComponent<Enemy>().SubscribeToDeath(StartDissolve);
    }

    public void StartDissolve()
    {
        StartCoroutine(Dissolve());
    }

    IEnumerator Dissolve()
    {
        GetComponent<Enemy>().enabled = false;
        Material mat = GetComponent<MeshRenderer>().material;
        mat.shader = dissolveShader;

        mat.SetFloat("_Visibility", 1);
        while (mat.GetFloat("_Visibility") > 0f)
        {
            mat.SetFloat("_Visibility", mat.GetFloat("_Visibility") - reduction);

            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
    }
}
