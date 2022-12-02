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

    // ó�� ���� ������ �ѹ� ȣ��.
    public virtual void OnEnterView()
    {
        if( m_btnClose != null )
        {
            m_btnClose.onClick.RemoveAllListeners();
            m_btnClose.onClick.AddListener(OnClickClose);
        }
    }

    // ��� �˾��� ��������� �ٽ� ȣ��.
    public virtual void OnStartView()
    {

    }

    // ��ܿ� �˾��� ��Ÿ������ ȣ��.
    public virtual void OnPauseView()
    {

    }

    // ������, �˾��� �������� ȣ��.
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
