using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWindow : BaseView
{
    private Game m_game;

    protected override void OnClickClose()
    {
        if( m_game != null )
            m_game.RemoveMap();

        WindowManager.Instance.ShowWindow(eWindow.WorldMapWindow);
    }

    public void SetInfo(int level)
    {
        if (m_game == null)
            m_game = gameObject.AddComponent<Game>();

        m_game.GameStart(level);
    }
}
