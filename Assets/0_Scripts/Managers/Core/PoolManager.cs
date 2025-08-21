using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IPool
{
	GameObject Original { get; set; }
	Transform Root { get; set; }
	Queue<GameObject> pool { get; set; }

	GameObject Get(Action<GameObject> action = null);

	void Return(GameObject obj, Action action = null);
}
public class Object_Pool : IPool
{
	public GameObject Original { get; set; }
	public Queue<GameObject> pool { get; set; } = new Queue<GameObject>();
	public Transform Root { get; set; }

	public GameObject Get(Action<GameObject> action = null)
	{
		GameObject obj = pool.Dequeue();
		obj.SetActive(true);
		action?.Invoke(obj);
		return obj;
	}

	public void Return(GameObject obj, Action action = null)
	{
		pool.Enqueue(obj);
		obj.transform.SetParent(Root);
		obj.SetActive(false);
		action?.Invoke();
	}

}

public class PoolManager
{

	public Dictionary<string, IPool> m_pool_Dictionary = new Dictionary<string, IPool>();
	Transform _root;

    public void Init()
    {
        if (_root == null)
        {
            _root = new GameObject { name = "@Pool_Root" }.transform;
            UnityEngine.Object.DontDestroyOnLoad(_root);
        }
    }

	public IPool Pooling_OBJ(string path)
	{
		if (m_pool_Dictionary.ContainsKey(path) == false)
		{
			Add_Pool(path);
		}

		if (m_pool_Dictionary[path].pool.Count <= 0) Add_Queue(path);
		return m_pool_Dictionary[path];
	}

	private GameObject Add_Pool(string path)
	{
		GameObject obj = new GameObject(path + "##POOL");
		obj.transform.SetParent(_root);
		Object_Pool T_Component = new Object_Pool();
		m_pool_Dictionary.Add(path, T_Component);
		T_Component.Root = obj.transform;
		return obj;
	}

	private void Add_Queue(string path)
	{
		//var go = Resources.Load<GameObject>(path);
		//go.transform.SetParent(m_pool_Dictionary[path].Root);
		//m_pool_Dictionary[path].Return(go);

		var prefab = Resources.Load<GameObject>(path);
		if (prefab == null)
		{
			Debug.LogError($"[PoolManager] Prefab not found at path: {path}");
			return;
		}
		var go = UnityEngine.Object.Instantiate(prefab);
		go.name = prefab.name; 

		m_pool_Dictionary[path].Original = prefab; 
		m_pool_Dictionary[path].Return(go);
	}

	public GameObject GetOriginal(string name)
    {
        if (m_pool_Dictionary.ContainsKey(name) == false)
            return null;
        return m_pool_Dictionary[name].Original;
    }

    public void Clear()
    {
        foreach (Transform child in _root)
            GameObject.Destroy(child.gameObject);

		m_pool_Dictionary.Clear();
    }
}
