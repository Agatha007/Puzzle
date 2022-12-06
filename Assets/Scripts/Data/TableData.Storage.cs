using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Table.Enum;
using Newtonsoft.Json;

public partial class TableData : MonoBehaviour
{
#if UNITY_EDITOR
    public bool IsLoadedTable = false;
#endif


    #region Wrapper
    [Serializable]
    public class Wrapper<T>
    {
        public T[] array;
    }
    #endregion


    #region Asset Mapping Path
    public class MappingPathData
    {
        public int assetId;
        public string assetPath;
    }
    public class AssetMappingPath
    {
        public List<MappingPathData> mappingPathDataList = new List<MappingPathData>();
    }
    Dictionary<int, string> AssetMappingPathDic = new Dictionary<int, string>();
    public class NpcPrefabData
    {
        public string prefabName;
        public string assetPath;
    }
    public class NpcPrefabPath
    {
        public List<NpcPrefabData> npcPrefabDataList = new List<NpcPrefabData>();
    }
    Dictionary<string, string> npcPrefabPathDic = new Dictionary<string, string>();
    #endregion Asset Mapping Path


    public void LoadResourcesTableData(Action callBack)
    {
        if( isFirstTableLoaded == false )
        {
            LoadTableResources<JStageData, int>(TABLEDATA.JStageData, "UID", (dic) => { masterDictionary.StageDataDic = dic; });
        }

        isFirstTableLoaded = true;
        callBack.Invoke();
    }

    private bool isFirstTableLoaded = false;

    private string GetJsonTableNameResource(TABLEDATA inType)
    {
        StringBuilder builder = new StringBuilder();

        builder.Append("IncludeTables/");
        builder.Append(inType.ToString());
        builder.Remove(builder.Length - 4, 4);

        return builder.ToString();
    }

    private FieldInfo GetFieldInfo(object obj, string variable)
    {
        return obj.GetType().GetField(variable);
    }

    private void LoadTableResources<T, P>(TABLEDATA inType, string keyString, UnityAction<Dictionary<P, T>> callback)
    {
        TextAsset text = Resources.Load<TextAsset>(GetJsonTableNameResource(inType));

        if (text == null)
        {
            Debug.LogError($"Load Table Resources : [{inType}] Null");

            callback?.Invoke(null);
            return;
        }   

        Wrapper<T> list = JsonConvert.DeserializeObject<Wrapper<T>>(text.text);
        Dictionary<P, T> dic = new Dictionary<P, T>();

        for (int i = 0; i < list.array.Length; ++i)
        {
            T dataobj = list.array[i];
            FieldInfo fid = GetFieldInfo(dataobj, keyString);

            P intKey = (P)fid.GetValue(dataobj);
            if (dic.ContainsKey(intKey))
            {
                Debug.LogWarning("LoadTable duplicatated key [" + inType + "] [" + intKey + "]");
            }
            else
            {
                dic.Add(intKey, dataobj);
            }
        }

        Debug.LogWarning("Loading Table [" + GetJsonTableNameResource(inType) + "] [" + dic.Count + "]");
        callback?.Invoke(dic);
    }
}
