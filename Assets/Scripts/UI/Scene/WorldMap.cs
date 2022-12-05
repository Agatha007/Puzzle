using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMap : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        WindowManager.Instance.ShowWindow(eWindow.WorldMapWindow);
    }
}
