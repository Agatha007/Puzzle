using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
	private static T instance = null;

	public static T Instance
	{
		get
		{
			if (instance == null)
			{
				instance = FindObjectOfType(typeof(T)) as T;
				if (instance == null)
				{
					GameObject go = new GameObject(typeof(T).ToString(), typeof(T));
					instance = go.GetComponent<T>();
					instance.Init();
				}
			}

			return instance;
		}
	}

	protected virtual void Awake()
	{
		if (instance == null)
		{
			instance = this as T;
			DontDestroyOnLoad(gameObject);
		}
	}

	public virtual void Init()
	{
	}

	protected virtual void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	protected virtual void OnApplicationQuit()
	{
		if (instance == this)
			instance = null;
	}
}
