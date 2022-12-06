using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

public class GameWindow : BaseView
{
    [SerializeField] private SpriteAtlas m_BlockAtlas = null;
    [SerializeField] private Image m_BlockImge;
    [SerializeField] private Text m_TxBlockCount;
    [SerializeField] private Text m_TxMoveCount;
    [SerializeField] private Text m_TxPoint;    

    private JStageData m_JStageData;

    private Game m_game;

    private int m_BlockCount;
    private int m_MoveCount;

    protected override void OnClickClose()
    {
        if( m_game != null )
            m_game.RemoveMap();

        WindowManager.Instance.ShowWindow(eWindow.WorldMapWindow);
    }

    public void SetInfo(int index)
    {
        if (m_game == null)
            m_game = gameObject.AddComponent<Game>();

        m_JStageData = GenDataMgr.Instance.DataTable.getStageDataList()[index];

        m_BlockCount = m_JStageData.ClearBlockCount;
        m_MoveCount = m_JStageData.MoveCount;

        m_BlockImge.sprite = m_BlockAtlas.GetSprite(m_JStageData.ClearBlockName);
        m_TxBlockCount.text = m_BlockCount.ToString();
        m_TxMoveCount.text = m_MoveCount.ToString();

        int level = index + 1;

        m_game.ACT_MOVE = MoveBlck;
        m_game.ACT_BLOCK_DELETE = BlockDelete;
        m_game.GameStart(level);
    }

    private void MoveBlck()
    {
        m_MoveCount--;

        if (m_MoveCount <= 0)
            m_MoveCount = 0;

        m_TxMoveCount.text = m_MoveCount.ToString();
    }

    private void BlockDelete(int index)
    {
        if( m_JStageData.ClearBlockIndex == index )
        {
            m_BlockCount--;

            if (m_BlockCount <= 0)
                m_BlockCount = 0;

            m_TxBlockCount.text = m_BlockCount.ToString();
        }
    }
}
