using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILoader<Key, Value>
{
    Dictionary<Key, Value> MakeDict();
}

public class DataManager
{
    //public Dictionary<int, Data.PlayerStat> StatDict { get; private set; } = new Dictionary<int, Data.Stat>();

    public Data.PlayerStat PlayerStat { get; private set; }
    public Data.GameSetting GameSetting { get; private set; }

	public Dictionary<string, string> DialogDict { get; private set; } = new Dictionary<string, string>();


	public void Init()
    {
        //StatDict = LoadJson<Data.StatData, int, Data.Stat>("StatData").MakeDict();

		PlayerStat = LoadJson<Data.PlayerStat>("PlayerStat");
		GameSetting = LoadJson<Data.GameSetting>("GameSetting");
		DialogDict = LoadJson<Data.DialogData, string, string>("Dialog_ko").MakeDict();


	}

	Loader LoadJson<Loader, Key, Value>(string path) where Loader : ILoader<Key, Value>
    {
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
        return JsonUtility.FromJson<Loader>(textAsset.text);
	}

	Loader LoadJson<Loader>(string path)
	{
		TextAsset textAsset = Managers.Resource.Load<TextAsset>($"Data/{path}");
		return JsonUtility.FromJson<Loader>(textAsset.text);
	}
}
