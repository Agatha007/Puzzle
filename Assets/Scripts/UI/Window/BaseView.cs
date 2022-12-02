using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseView : MonoBehaviour
{
    [SerializeField] protected Button m_btnClose;

    private string _viewName;
    public string ViewName { get { return _viewName; } set { _viewName = value; } }

    public eViewType ViewType { get; set; }

    private Action<GameObject> m_actCloseUseManager;
    public Action<GameObject> ACT_CLOSE_USE_MANAGER { set { m_actCloseUseManager = value; } }

    private Action m_actClose;
    public Action ACT_CLOSE { set { m_actClose = value; } }

    private void Awake()
    {
        
    }

    protected virtual void Start()
    {

    }

    // 처음 진입 했을때 한번 호출.
    public virtual void OnEnterView()
    {
        if( m_btnClose != null )
        {
            m_btnClose.onClick.RemoveAllListeners();
            m_btnClose.onClick.AddListener(OnClickClose);
        }
    }

    // 모든 팝업이 사라졌을때 다시 호출.
    public virtual void OnStartView()
    {

    }

    // 상단에 팝업이 나타났을때 호출.
    public virtual void OnPauseView()
    {

    }

    // 윈도우, 팝업이 닫혔을때 호출.
    public virtual void OnExitView()
    {
        if (m_actClose != null) m_actClose();
        if (gameObject != null)
            Destroy(gameObject);
    }

    protected virtual void OnClickClose()
    {
        if (m_actCloseUseManager != null) 
            m_actCloseUseManager(gameObject);
    }
}
