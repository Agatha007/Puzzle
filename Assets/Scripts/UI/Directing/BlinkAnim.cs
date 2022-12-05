using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlinkAnim : MonoBehaviour
{
    [Range(0f, 1f)] public float from = 1f;
    [Range(0f, 1f)] public float to = 1f;
    [Range(0f, 1f)] public float Duration = 0.5f;

    private Text m_Text;
    private Color m_Color;
    private float m_time;

    private void OnEnable()
    {
        SetInfo();
    }

    private void Update()
    {
        if( m_Text != null )
        {
            if( m_time < Duration)
            {
                m_Text.color = new Color(m_Color.r, m_Color.g, m_Color.b, 1 - m_time);
            }
            else
            {
                m_Text.color = new Color(m_Color.r, m_Color.g, m_Color.b, m_time);

                if (m_time > to)
                    m_time = from;
            }

            m_time += Time.deltaTime;
        }
    }

    void SetInfo()
    {
        if( m_Text == null )
        {
            m_Text = GetComponent<Text>();
            m_Color = m_Text.color;
            m_Color.a = from;
        }   
    }
}
