using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    public int m_Index;
    public int m_BlockIndex;

    public Image m_Image;

    public BLOCK_CREATE_TYPE m_CreateType;

    private Block m_DownTaget;
    private float m_MoveTime = 0.2f;

    private Action<Block> m_ActionFinish;
    private IEnumerator m_IEMove;

    public void Move(float time, Action<Block> actionFinish = null)
    {
        m_MoveTime = time;
        m_ActionFinish = actionFinish;

        if (m_IEMove != null)
            StopCoroutine(m_IEMove);

        StartCoroutine(m_IEMove = IEMove());
    }

    IEnumerator IEMove()
    {
        if( m_Image != null )
        {
            var moveTime = 0.0f;

            RectTransform rtBlock = gameObject.GetComponent<RectTransform>();
            RectTransform rtImage = m_Image.GetComponent<RectTransform>();

            Vector2 startPos = rtImage.anchoredPosition;
            Vector2 endPos = rtBlock.anchoredPosition;

            while (moveTime < m_MoveTime)
            {
                moveTime += Time.deltaTime;

                if( rtImage != null )
                    rtImage.anchoredPosition = Vector3.Lerp(startPos, endPos, moveTime / m_MoveTime);

                yield return null;
            }

            m_ActionFinish?.Invoke(this);
        }
    }
}
