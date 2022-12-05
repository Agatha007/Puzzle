using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WorldMapWindow : BaseView
{
    [SerializeField] List<Button> m_StageList = new List<Button>();

    public override void OnEnterView()
    {
        base.OnEnterView();

        for( int i=0; i< m_StageList.Count; i++ )
        {
            int level = i + 1;
            m_StageList[i].onClick.RemoveAllListeners();
            m_StageList[i].onClick.AddListener(() => OnClickStage(level));
        }
    }

    public void OnClickStage(int level)
    {
        WindowManager.Instance.ShowWindow(eWindow.GameWindow, (view) =>
        {
            if( view != null )
            {
                GameWindow window = (GameWindow)view;

                if( window != null )
                    window.SetInfo(level);
            }
        });
    }
}
;