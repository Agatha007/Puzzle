using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.U2D;

[Serializable]
public class BlockList
{
    public List<Block> m_blocks;
}

public class Map : MonoBehaviour
{
    [SerializeField] private GAME_STATE GameState = GAME_STATE.NONE;
    [SerializeField] private Transform m_ImageParent = null;
    [SerializeField] private SpriteAtlas m_Atlas = null;

    [SerializeField] private List<BlockList> m_BlockList_Down = null;    
    [SerializeField] private List<BlockList> m_BlockList_RightDown = null;
    [SerializeField] private List<BlockList> m_BlockList_LeftDown = null;

    public List<Block> m_MoveBlocks = new List<Block>();    

    public GAME_STATE m_GameState { set { GameState = value; } get { return GameState; } }

    private Action<int> m_actBlockDelete;
    public Action<int> ACT_BLOCK_DELETE { get { return m_actBlockDelete; } set { m_actBlockDelete = value; } }

    #region BLOCK SETTING    
    // 블럭 셋팅.
    public void SetBlock()
    {
        int index = 0;
        for (int i = 0; i < m_BlockList_Down.Count; i++)
        {
            BlockList blockList = m_BlockList_Down[i];
            for (int j = 0; j < blockList.m_blocks.Count; j++)
            {
                GameObject obj = transform.Find($"Block/block{index:D2}").gameObject;

                Block block = obj.GetComponent<Block>();
                block.m_Index = index;

                index++;
            }
        }
    }

    // 블럭 이미지 생성.
    public void SetBlockImage()
    {
        for (int i = 0; i < m_BlockList_Down.Count; i++)
        {
            BlockList blockList = m_BlockList_Down[i];
            for (int j = 0; j < blockList.m_blocks.Count; j++)
            {
                if (blockList.m_blocks[j].m_Image != null)
                    DestroyImmediate(blockList.m_blocks[j].m_Image.gameObject);
            }
        }

        for (int i = 0; i < m_BlockList_Down.Count; i++)
        {
            BlockList blockList = m_BlockList_Down[i];
            for (int j = 0; j < blockList.m_blocks.Count; j++)
            {
                GameObject obj = Instantiate(Resources.Load("BlockImage") as GameObject);

                obj.transform.SetParent(m_ImageParent);
                obj.transform.position = blockList.m_blocks[j].transform.position;

                RectTransform rt = obj.GetComponent<RectTransform>();
                rt.localScale = Vector3.one;

                int index = blockList.m_blocks[j].m_BlockIndex;

                if (index == 0)
                    index = UnityEngine.Random.Range(1, GameData.m_BlockName.Length + 1);

                Image image = obj.GetComponent<Image>();
                image.sprite = m_Atlas.GetSprite(GameData.m_BlockName[index - 1]);

                blockList.m_blocks[j].m_Image = image;
                blockList.m_blocks[j].m_BlockIndex = index;
            }
        }
    }
    #endregion BLOCK SETTING

    #region BLOCK CHANGE
    // 선택한 두개의 블럭이 이동 가능한지 체크 후 이동 시킨다.
    public IEnumerator Move(Action actionFinish)
    {
        yield return StartCoroutine(IEMove());

        // false : 블럭이 이동 후 맞는게 없을때 다시 원래대로 돌린다.
        if (BlockCheck() == false)
        {
            yield return StartCoroutine(IEMove());

            m_GameState = GAME_STATE.START;
        }
        else
        {
            yield return StartCoroutine(BlockDownMove());

            actionFinish?.Invoke();
        }
    }

    // 선택한 두개의 블럭을 이동한다.
    IEnumerator IEMove()
    {
        m_GameState = GAME_STATE.MOVE;

        var moveTime = 0.0f;

        Block moveMoveBlock1 = m_MoveBlocks[0];
        Block moveMoveBlock2 = m_MoveBlocks[1];

        Image Image1 = moveMoveBlock1.m_Image;
        Image Image2 = moveMoveBlock2.m_Image;

        int Index1 = moveMoveBlock1.m_BlockIndex;
        int Index2 = moveMoveBlock2.m_BlockIndex;

        RectTransform rtMoveBlock1 = moveMoveBlock1.GetComponent<RectTransform>();
        RectTransform rtMoveBlock2 = moveMoveBlock2.GetComponent<RectTransform>();

        RectTransform rtMoveBlockImage1 = Image1.GetComponent<RectTransform>();
        RectTransform rtMoveBlockImage2 = Image2.GetComponent<RectTransform>();

        while (moveTime < GameData.m_OneBlockMoveTime)
        {
            moveTime += Time.deltaTime;

            rtMoveBlockImage1.anchoredPosition = Vector3.Lerp(rtMoveBlock1.anchoredPosition, rtMoveBlock2.anchoredPosition, moveTime / GameData.m_OneBlockMoveTime);
            rtMoveBlockImage2.anchoredPosition = Vector3.Lerp(rtMoveBlock2.anchoredPosition, rtMoveBlock1.anchoredPosition, moveTime / GameData.m_OneBlockMoveTime);

            yield return null;
        }

        m_MoveBlocks[0].m_Image = Image2;
        m_MoveBlocks[1].m_Image = Image1;

        m_MoveBlocks[0].m_BlockIndex = Index2;
        m_MoveBlocks[1].m_BlockIndex = Index1;        
    }
    #endregion BLOCK MOVE  

    #region BLOCK DOWN MOVE   

    // 블럭들을 빈칸에 체운다.
    public IEnumerator BlockDownMove()
    {
        m_GameState = GAME_STATE.MOVE;

        List<Block> blocks = new List<Block>();
        for (int i = 0; i < m_BlockList_Down.Count; i++)
        {
            BlockList blockList = m_BlockList_Down[i];

            for (int j = blockList.m_blocks.Count-1; j >= 0; j--)
            {
                Block block = blockList.m_blocks[j];

                if (block.m_Image == null)
                {
                    for (int k = j; k >= 0; k--)
                    {
                        if (blockList.m_blocks[k].m_Image != null)
                        {
                            block.m_Image = blockList.m_blocks[k].m_Image;
                            blockList.m_blocks[k].m_Image = null;

                            block.m_BlockIndex = blockList.m_blocks[k].m_BlockIndex;
                            blockList.m_blocks[k].m_BlockIndex = 0;

                            blocks.Add(block);

                            block = blockList.m_blocks[k];
                            break;
                        }
                    }                 
                }
            }
        }

        for( int i=0; i< blocks.Count; i++ )
            blocks[i].Move(GetBlockMoveTime(blocks[i]));

        CreateBlockCheck(() =>
        {
            StartCoroutine(BlockMoveEnd());
        });

        yield return null;
    }

    // 블럭의 상태를 다시 체크한다.
    private IEnumerator BlockMoveEnd()
    {
        yield return new WaitForSeconds(0.2f);

        if (BlockCheck())
            StartCoroutine(BlockDownMove());
        else
            m_GameState = GAME_STATE.START;
    }

    #endregion BLOCK DOWN MOVE   

    #region BLOCK CHECK
    // 삭제 가능한 블럭이 있는지 체크.
    public bool BlockCheck()
    {
        bool isCheck = false;
        if (BlockDownCheck())       isCheck = true;
        if (BlockRightUpCheck())    isCheck = true;
        if (BlockRightDownCheck())  isCheck = true;        

        return isCheck;
    }

    private bool BlockDownCheck()
    {
        List<Block> blocks = new List<Block>();
        for (int i = 0; i < m_BlockList_Down.Count; i++)
        {
            BlockList blockList = m_BlockList_Down[i];
            for (int j = 1; j < blockList.m_blocks.Count - 1; j++)
            {
                Block blockUp = blockList.m_blocks[j - 1];
                Block blockCenter = blockList.m_blocks[j];
                Block blockDown = blockList.m_blocks[j + 1];

                if (blockCenter.m_BlockIndex == 0)
                    continue;

                if (blockCenter.m_BlockIndex == blockUp.m_BlockIndex &&
                    blockCenter.m_BlockIndex == blockDown.m_BlockIndex)
                {
                    if (blocks.Find(x => x.m_Index == blockUp.m_Index) == false)
                        blocks.Add(blockUp);
                    if (blocks.Find(x => x.m_Index == blockCenter.m_Index) == false)
                        blocks.Add(blockCenter);
                    if (blocks.Find(x => x.m_Index == blockDown.m_Index) == false)
                        blocks.Add(blockDown);
                }
            }
        }

        if (blocks.Count >= 3)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].m_Image != null && blocks[i].m_Image.gameObject != null)
                {
                    m_actBlockDelete?.Invoke(blocks[i].m_BlockIndex);

                    DestroyImmediate(blocks[i].m_Image.gameObject);                   
                }

                blocks[i].m_BlockIndex = 0;
            }
        }

        return blocks.Count >= 3;
    }

    private bool BlockRightUpCheck()
    {
        List<Block> blocks = new List<Block>();
        for (int i = 0; i < m_BlockList_LeftDown.Count; i++)
        {
            BlockList blockList = m_BlockList_LeftDown[i];
            for (int j = 1; j < blockList.m_blocks.Count-1; j++)
            {
                Block blockLeft = blockList.m_blocks[j - 1];
                Block blockCenter = blockList.m_blocks[j];
                Block blockRight = blockList.m_blocks[j + 1];

                if (blockCenter.m_BlockIndex == -1 || blockLeft == null || blockRight == null)
                    continue;

                if (blockCenter.m_BlockIndex == blockLeft.m_BlockIndex &&
                    blockCenter.m_BlockIndex == blockRight.m_BlockIndex)
                {
                    if (blocks.Find(x => x.m_Index == blockLeft.m_Index) == false)
                        blocks.Add(blockLeft);
                    if (blocks.Find(x => x.m_Index == blockCenter.m_Index) == false)
                        blocks.Add(blockCenter);
                    if (blocks.Find(x => x.m_Index == blockRight.m_Index) == false)
                        blocks.Add(blockRight);
                }
            }
        }

        if (blocks.Count >= 3)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].m_Image != null && blocks[i].m_Image.gameObject != null)
                {
                    m_actBlockDelete?.Invoke(blocks[i].m_BlockIndex);

                    DestroyImmediate(blocks[i].m_Image.gameObject);
                }

                blocks[i].m_BlockIndex = 0;
            }
        }

        return blocks.Count >= 3;
    }

    private bool BlockRightDownCheck()
    {
        List<Block> blocks = new List<Block>();

        for (int i = 0; i < m_BlockList_RightDown.Count; i++)
        {
            BlockList blockList = m_BlockList_RightDown[i];
            for (int j = 1; j < blockList.m_blocks.Count-1; j++)
            {
                Block blockLeft = blockList.m_blocks[j - 1];
                Block blockCenter = blockList.m_blocks[j];
                Block blockRight = blockList.m_blocks[j + 1];

                if (blockCenter.m_BlockIndex == -1 || blockLeft == null || blockRight == null)
                    continue;

                if (blockCenter.m_BlockIndex == blockLeft.m_BlockIndex &&
                    blockCenter.m_BlockIndex == blockRight.m_BlockIndex)
                {
                    if (blocks.Find(x => x.m_Index == blockLeft.m_Index) == false)
                        blocks.Add(blockLeft);
                    if (blocks.Find(x => x.m_Index == blockCenter.m_Index) == false)
                        blocks.Add(blockCenter);
                    if (blocks.Find(x => x.m_Index == blockRight.m_Index) == false)
                        blocks.Add(blockRight);
                }
            }
        }

        if (blocks.Count >= 3)
        {
            for (int i = 0; i < blocks.Count; i++)
            {
                if (blocks[i].m_Image != null && blocks[i].m_Image.gameObject != null)
                {
                    m_actBlockDelete?.Invoke(blocks[i].m_BlockIndex);

                    DestroyImmediate(blocks[i].m_Image.gameObject);
                }

                blocks[i].m_BlockIndex = 0;
            }
        }

        return blocks.Count >= 3;
    }
    #endregion

    #region BLANK BLOCK MOVE
    // 생성 할 수 있는 블럭 체크 후 생성 및 이동.
    private void CreateBlockCheck(Action actionFinish = null)
    {
        int endCount = 0;

        for (int i = 0; i < m_BlockList_Down.Count; i++)
        {
            StartCoroutine(CreateDownBlock(m_BlockList_Down[i], () =>
            {
                endCount++;

                if(endCount == m_BlockList_Down.Count)
                    actionFinish?.Invoke();
            }));
        }
    }

    // 블럭을 생성 후 아래로 이동.
    private IEnumerator CreateDownBlock(BlockList blockList, Action actionFinish)
    {
        while(true)
        {
            List<Block> blocks = CreateBlockList(blockList);

            for (int i = blocks.Count - 1; i >= 0; i--)
            {
                BlockMoveDown(blocks[i]);

                yield return new WaitForSeconds(GameData.m_OneBlockMoveTime + 0.1f);
            }

            if (blocks.Count == 0)
                break;
        }

        actionFinish?.Invoke();
    }

    // 블럭을 아래로 이동 시킨다.
    private void BlockMoveDown(Block block)
    {
        if (block.m_Image != null)
            block.m_Image.gameObject.SetActive(true);

        block.Move(GetBlockMoveTime(block));
    }

    // 블럭 내려 오는 시간 계산.
    private float GetBlockMoveTime(Block curBlock)
    {
        float time = GameData.m_OneBlockMoveTime;

        RectTransform rtBlock = curBlock.GetComponent<RectTransform>();

        if( curBlock.m_Image != null )
        {
            RectTransform rtImage = curBlock.m_Image.GetComponent<RectTransform>();

            float di = Vector2.Distance(rtBlock.anchoredPosition, rtImage.anchoredPosition);
            time = (di / 80) * GameData.m_OneBlockMoveTime;
        }

        return time;
    }

    // 생성될 블럭.
    private List<Block> CreateBlockList(BlockList blockList)
    {
        List<Block> blocks = blockList.m_blocks.FindAll(x => x.m_Image == null);
        for (int i = blocks.Count - 1; i >= 0; i--)
        {
            if (blocks[0].m_CreateType == BLOCK_CREATE_TYPE.NO_CREATE)
                break;

            BlockCreate(blocks, blocks[i]);
        }

        return blocks;
    }

    // 블럭 생성.
    private void BlockCreate(List<Block> blocklist, Block block)
    {
        if (block.m_Image == null)
        {
            GameObject obj = Instantiate(Resources.Load("BlockImage") as GameObject);
            obj.transform.SetParent(m_ImageParent);

            RectTransform blockRt = blocklist[0].GetComponent<RectTransform>();
            Vector3 anchoredPosition = blockRt.anchoredPosition;
            anchoredPosition.y += 80;

            RectTransform rt = obj.GetComponent<RectTransform>();
            rt.localScale = Vector3.one;
            rt.anchoredPosition = anchoredPosition;

            int index = UnityEngine.Random.Range(1, GameData.m_BlockName.Length + 1);

            Image image = obj.GetComponent<Image>();
            image.sprite = m_Atlas.GetSprite(GameData.m_BlockName[index - 1]);

            block.m_Image = image;
            block.m_BlockIndex = index;

            block.m_Image.gameObject.SetActive(false);
        }
    }
    #endregion BLANK BLOCK MOVE
}