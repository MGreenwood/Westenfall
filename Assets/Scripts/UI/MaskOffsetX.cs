using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaskOffsetX : MonoBehaviour
{

    [SerializeField]
    float speed = 10f;
    Material mat;

    float offset = 0f;
    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponent<Image>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset = speed * Time.time;
        mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));

        if (offset > 128)
            offset = 0f;
    }
}
