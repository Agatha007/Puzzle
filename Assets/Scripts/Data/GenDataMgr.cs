using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenDataMgr : Singleton<GenDataMgr>
{
    public bool IsInitialize { get; private set; } = false;

    public TableData DataTable = null;

    protected override void Awake()
    {
        base.Awake();

        if (DataTable == null)
            DataTable = this.gameObject.AddComponent<TableData>();
    }

    //public IEnumerator InitAsync(UnityAction endCallback)
    //{
    //    StartCoroutine(DataTable.Load(() =>
    //    {
    //        IsInitialize = true;
    //    }));

    //    yield return new WaitUntil(() => IsInitialize);
    //    endCallback?.Invoke();
    //}
}
