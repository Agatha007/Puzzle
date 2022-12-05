using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : MonoBehaviour
{
    [SerializeField] private Button m_BtnTouch;

    // Start is called before the first frame update
    void Start()
    {
        if( m_BtnTouch != null )
        {
            m_BtnTouch.onClick.RemoveAllListeners();
            m_BtnTouch.onClick.AddListener(OnClickStart);
        }
    }

    void OnClickStart()
    {
        PlayState.Instance.ChangePlayState(PlayState.STATES.WorldMap);
    }
}
