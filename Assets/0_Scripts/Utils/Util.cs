using System;
using UnityEngine;

public class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();
        if (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        if (transform == null)
            return null;

        return transform.gameObject;
    }

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
            return null;

        if (recursive == false) {
            for (int i = 0; i < go.transform.childCount; i++) {
                Transform transform = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(name) || transform.name == name) {
                    T component = transform.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else {
            foreach (T component in go.GetComponentsInChildren<T>()) {
                if (string.IsNullOrEmpty(name) || component.name == name)
                    return component;
            }
        }

        return null;
    }

    static public int MakeRockId(int depth, int index) => depth * 100 + index;
    static public int MakeVeinId(int depth, int index) => depth * 100 + (index + 1) * 10;

    static public string MakeHexRockId(int depth, int index) => GetHex(MakeRockId(depth, index));
    static public string MakeHexVeinId(int depth, int index) => GetHex(MakeVeinId(depth, index));

    static public int GetDec(string id) => Convert.ToInt32(id, 16);
    static public string GetHex(int id) => id.ToString("X");
}
