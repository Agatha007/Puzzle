using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : Singleton<WindowManager>
{
    [SerializeField] private Canvas _Canvas = null;
    public Canvas m_Canvas
    {
        get
        {
            if( _Canvas == null )
                _Canvas = GameObject.Find("Canvas").GetComponent<Canvas>();

            return _Canvas;
        }
    }

    [SerializeField] private Transform _windowParent = null;
    public Transform PARENT_WINDOW
    {
        get
        {
            if (m_Canvas != null && _windowParent == null )
                _windowParent = m_Canvas.transform.Find("WindowParent");

            return _windowParent;
        }
    }

    [SerializeField] private Transform _popupParent = null;
    public Transform PARENT_POPUP
    {
        get
        {
            if (m_Canvas != null && _popupParent == null)
                _popupParent = _Canvas.transform.Find("PopupParent");

            return _popupParent;
        }
    }

    [SerializeField] private Transform _popupSystemParent = null;
    public Transform PARENT_POPUP_SYSTEM
    {
        get
        {
            if (m_Canvas != null && _popupSystemParent == null)
                _popupSystemParent = m_Canvas.transform.Find("SystemPopupParent");

            return _popupSystemParent;
        }
    }

    [SerializeField] private Camera m_Camera = null;
    public Camera UI_CAMERA
    {
        get
        {
            if (m_Camera == null)
                m_Camera = GameObject.Find("Main Camera").GetComponent<Camera>();

            return m_Camera;
        }
    }

    private string POPUP = "Popup/{0}";
    private string WINDOW = "Window/{0}";

    public List<BaseView> ViewList = new List<BaseView>();

    void Update()
    {
        if( Input.GetKeyDown(KeyCode.Escape) )
        {

        }
    }

    public void ShowPopup(ePopup popup, Action<BaseView> completeCallback = null)
    {
        if (GetCurrentViewName().Equals( popup.ToString() ))
            return;

        GetPrefab(POPUP, popup.ToString(), PARENT_POPUP, (loadObj) =>
        {
            SetView(eViewType.Popup, popup.ToString(), loadObj, (loadView) =>
            {
                completeCallback?.Invoke(loadView);
            });
        });
    }

    public void ShowWindow(eWindow window, Action<BaseView> completeCallback = null)
    {
        if (GetCurrentViewName().Equals(window.ToString()))
            return;

        GetPrefab(WINDOW, window.ToString(), PARENT_WINDOW, (loadObj) =>
        {
            SetView(eViewType.Window, window.ToString(), loadObj, (loadView) =>
            {
                completeCallback?.Invoke(loadView);
            });
        });
    }

    private void SetView(eViewType type, string viewNaame, GameObject loadObj, Action<BaseView> completeCallback = null)
    {
        BaseView loadView = loadObj.GetComponent<BaseView>();

        loadView.ViewType = type;
        loadView.ViewName = viewNaame;
        loadView.ACT_CLOSE_USE_MANAGER = ClosePopup;
        CallPauseCurrentPopup();
        ViewList.Add(loadView);
        loadView.OnEnterView();
        loadView.OnStartView();

        completeCallback?.Invoke(loadView);
    }

    private void GetPrefab(string _path, string prefabName, Transform parent, Action<GameObject> completeCallback = null)
    {
        string _fullPath = string.Format(_path, prefabName);

        GameObject obj = Instantiate( Resources.Load(_fullPath) ) as GameObject;

        if (obj == null)
        {
            Debug.LogError("PREFAB CREATE FAILED :  [" + prefabName + "]");
            return;
        }

        obj.name = prefabName;
        obj.transform.parent = parent;
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;

        RectTransform rect = obj.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.sizeDelta = Vector2.zero;
        }

        completeCallback?.Invoke(obj);
    }

    private void CallPauseCurrentPopup()
    {
        BaseView curPopup = GetCurrentPopup();
        if (curPopup != null)
            curPopup.OnPauseView();
    }

    public BaseView GetCurrentPopup()
    {
        if (ViewList != null && ViewList.Count > 0)
        {
            for (int i = ViewList.Count - 1; i >= 0; i--)
            {
                if (ViewList[i] != null)
                {
                    BaseView _baseView = ViewList[i];
                    if (_baseView != null)
                        return _baseView;
                }
            }
        }
        return null;
    }

    public void ClosePopup(GameObject popup)
    {
        if (popup == null) return;
        if (ViewList != null && ViewList.Count > 0)
        {
            for (int i = ViewList.Count - 1; i >= 0; i--)
            {
                if (ViewList[i] != null && ViewList[i].gameObject != null)
                {
                    if (ViewList[i].gameObject.GetInstanceID() == popup.GetInstanceID())
                    {
                        BaseView _popup = ViewList[i];

                        if (ViewList.Count > i)
                            ViewList.RemoveAt(i);

                        if (_popup != null && _popup.gameObject != null)
                            _popup.OnExitView();

                        RefreshCurrentView();
                        return;
                    }
                }
            }
        }
    }

    private void RefreshCurrentView()
    {
        if (PARENT_POPUP == null) return;

        if (ViewList != null && ViewList.Count > 0)
        {
            BaseView _curView = ViewList[ViewList.Count - 1];
            if (_curView != null)
            {
                if (_curView.ViewType == eViewType.Popup)
                    CallRefreshCurrentPopup();
                else if (_curView.ViewType == eViewType.Window)
                    SetWindowActive(_curView, true);
            }
        }
    }

    private void CallRefreshCurrentPopup()
    {
        BaseView _popup = GetCurrentPopup();
        if (_popup != null)
        {
            _popup.gameObject.SetActive(true);
            _popup.OnStartView();
        }
    }

    void SetWindowActive(BaseView view, bool inActive, bool isEnter = false)
    {
        if (inActive)
        {
            view.gameObject.SetActive(inActive);
            if (isEnter)
                view.OnEnterView();
            else
                view.OnStartView();
        }
        else
        {
            view.gameObject.SetActive(inActive);
        }
    }

    private string GetCurrentViewName()
    {
        if (ViewList != null && ViewList.Count > 0)
        {
            for (int i = ViewList.Count - 1; i >= 0; i--)
            {
                if (ViewList[i] != null)
                {
                    BaseView _baseView = ViewList[i];
                    if (_baseView != null)
                        return _baseView.ViewName;
                }
            }
        }

        return string.Empty;
    }
}
