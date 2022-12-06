using System.Collections;
using System.Collections.Generic;
using System.Linq;

public partial class TableData
{
    private TableMasterData masterDictionary = new TableMasterData();

    #region Stage
    public List<JStageData> getStageDataList() { return masterDictionary.StageDataDic.Values.ToList(); }
    #endregion Stage
}
