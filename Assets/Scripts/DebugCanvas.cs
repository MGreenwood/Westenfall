using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugCanvas : MonoBehaviour
{
    Canvas canvas;

    [SerializeField]
    Text t_mousePos;

    // FPS 
    const float fpsMeasurePeriod = 0.5f;
    private int m_FpsAccumulator = 0;
    private float m_FpsNextPeriod = 0;
    private int m_CurrentFps;
    const string display = "{0} FPS";
    [SerializeField]
    private Text fpsText;
    // End fps

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
        m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.Debug)
        {
            if (!canvas.enabled)
                canvas.enabled = true;

            Vector2 mousePos = Input.mousePosition;

            t_mousePos.text = string.Format("Mouse position - x: {0} y: {1}", mousePos.x.ToString(), mousePos.y.ToString());


            // measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int)(m_FpsAccumulator / fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;
                fpsText.text = string.Format(display, m_CurrentFps);
            }
            // fps end
        }
        else
            canvas.enabled = false;
    }
}
