using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;
using UnityEngine.Experimental.U2D;

public class Game : MonoBehaviour
{
    [SerializeField] private Camera m_Cam = null;    
    [SerializeField] private Transform m_MapParent = null;

    [SerializeField] private int m_StageIndex;

    private Map m_Map;

    private IEnumerator IEStart;

    // Start is called before the first frame update
    void Start()
    {
        if (IEStart != null)
            StopCoroutine(IEStart);

        StartCoroutine(IEStart = GameStart());
    }

    // Update is called once per frame
    void Update()
    {
        TouchEvent();
        KeyEvent();
    }

    #region TOUCH EVENT
    private void TouchEvent()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ReGameStart();
        }

        if (Input.GetMouseButtonDown(0))
        {
            m_Map.m_MoveBlocks.Clear();

            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
                m_Map.m_MoveBlocks.Add(hit.transform.GetComponent<Block>());
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = m_Cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (m_Map.m_MoveBlocks.Count >= 2)
                    return;

                if (m_Map.m_MoveBlocks.Count > 0)
                {
                    if (!m_Map.m_MoveBlocks[0].name.Equals(hit.transform.name))
                        m_Map.m_MoveBlocks.Add(hit.transform.GetComponent<Block>());
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (m_Map.m_MoveBlocks.Count >= 2)
            {
                if (m_Map.m_MoveBlocks[0].m_Image == null || m_Map.m_MoveBlocks[1].m_Image == null)
                    return;

                if ( m_Map.m_GameState == GAME_STATE.START && IsTouchBlock() )
                    StartCoroutine( m_Map.Move() );
            }
        }
    }

    private bool IsTouchBlock()
    {
        if (m_Map.m_MoveBlocks.Count >= 2)
        {
            Vector2 startPos = m_Map.m_MoveBlocks[0].transform.position;
            Vector2 endPos = m_Map.m_MoveBlocks[1].transform.position;

            float value = Vector3.Distance(startPos, endPos);

            if( value < 0.7f) return true;
        }

        return false;
    }

    #endregion TOUCH EVENT

    #region KEY EVENT
    private bool m_IsEscape = false;
    private int m_EscapeValue;
    private float m_EscapeTime = 0f;

    private void KeyEvent()
    {
        #region Application Quit
        if ( m_IsEscape )
        {
            m_EscapeTime += Time.deltaTime;

            if (m_EscapeTime >= 2f)
            {
                m_IsEscape = false;
                m_EscapeValue = 0;
                m_EscapeTime = 0f;
            }   
        }

        if( Input.GetKeyDown(KeyCode.Escape) )
        {
            m_IsEscape = true;
            m_EscapeValue++;

            if( m_EscapeValue >= 2 )
                Application.Quit();
        }
        #endregion
    }
    #endregion KEY EVENT

    #region GAME SETTING
    private void SetMap()
    {
        if (m_Map != null)
        {
            Destroy(m_Map.gameObject);
            m_Map = null;
        }   

        if( m_Map == null )
        {
            GameObject obj = Instantiate(Resources.Load($"Map/Map{m_StageIndex:D3}") as GameObject);
            obj.transform.SetParent( m_MapParent.transform );

            obj.transform.localPosition = Vector3.zero;
            obj.transform.localScale = Vector3.one;

            m_Map = obj.GetComponent<Map>();
        }

        m_Map.SetBlock();
        m_Map.SetBlockImage();
    }

    private IEnumerator GameStart()
    {
        yield return new WaitForSeconds(1f);

        SetMap();

        m_Map.m_GameState = GAME_STATE.START;

        if (m_Map.BlockCheck())
        {
            StartCoroutine(m_Map.BlockDownMove());
        }
    }

    private void ReGameStart()
    {
        if (m_Map.m_GameState == GAME_STATE.MOVE)
            return;

        if (IEStart != null)
            StopCoroutine(IEStart);

        StartCoroutine(IEStart = GameStart());
    }
    #endregion BLOCK SETTING   
}