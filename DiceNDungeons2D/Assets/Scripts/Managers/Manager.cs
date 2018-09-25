using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager<T> : MonoBehaviour where T : Manager<T>
{
	private static T instance;
	private static string className = string.Empty;
	public static T Instance
	{
		get
		{
			if (instance != null)
				return instance;

			instance = FindObjectOfType<T>();
			className = typeof(T).ToString();

			if (instance == null)
				Debug.LogWarning("Could not find manager of type " + className);

			return instance;
		}
	}
    
	public virtual void Initialize()
	{
		if (instance != null)
			return;

		instance = FindObjectOfType(typeof(T)) as T;
		className = typeof(T).ToString();
	}
	
	
}