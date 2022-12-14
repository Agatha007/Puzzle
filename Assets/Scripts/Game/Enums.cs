using System;

public enum GAME_STATE
{
    NONE,
    START,
    CHECK,
    MOVE
}

public enum BLOCK_CREATE_TYPE
{
    NONE,
    NO_CREATE,   // 블럭 생성 불가.
}

public enum ePopup
{
    NONE,
}

public enum eWindow
{
    NONE,
    GameWindow,
    WorldMapWindow,
}

public enum eViewType
{
    Popup,
    Window,
}