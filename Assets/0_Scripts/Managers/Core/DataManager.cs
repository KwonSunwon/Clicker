using Data;
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

		//PlayerStat = LoadJson<Data.PlayerStat>("PlayerStat");
		//GameSetting = LoadJson<Data.GameSetting>("GameSetting");
		//DialogDict = LoadJson<Data.DialogData, string, string>("Dialog_ko").MakeDict();


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

	public static Dictionary<int, SkillNodeData> BuildFromResources(string path = "Data/SkillDatabase")
	{
		var dict = new Dictionary<int, SkillNodeData>();

		var json = Resources.Load<TextAsset>(path);
		if (json == null)
		{
			Debug.LogError($"[SkillDB] Not found: Resources/{path}.json");
			return dict;
		}

		var db = JsonUtility.FromJson<SkillDatabaseDto>(json.text);
		if (db?.skills == null)
		{
			Debug.LogError("[SkillDB] Parse failed.");
			return dict;
		}

		foreach (var dto in db.skills)
		{
			if (dict.ContainsKey(dto.id))
			{
				Debug.LogWarning($"[SkillDB] Duplicate id {dto.id} skipped.");
				continue;
			}

			var node = new SkillNodeData
			{
				Id = dto.id,
				Name = dto.name,
				Description = dto.description,
				MaxLevel = dto.maxLevel > 0 ? dto.maxLevel : 1,
				SkillCost = new List<(MineralType mineralType, BigNumber cost)>(),
				precedingSkills = dto.precedingSkills ?? new List<int>(),
				Level = 0,
				Xpos = dto.xPos,
				Ypos = dto.yPos,
				Edges = dto.Edges ?? new List<int>()
			};

			if (dto.skillCost != null)
			{
				foreach (var c in dto.skillCost)
				{
					if (!System.Enum.TryParse(c.type, out MineralType mt))
					{
						Debug.LogWarning($"[SkillDB] Unknown MineralType '{c.type}' in skill {dto.id}");
						continue;
					}
					node.SkillCost.Add((mt, new BigNumber(c.value)));
				}
			}

			dict.Add(node.Id, node);
		}

		// 간단한 선행스킬 검증(없는 id 참조 경고)
		foreach (var kv in dict)
		{
			foreach (var pre in kv.Value.precedingSkills)
			{
				if (!dict.ContainsKey(pre))
					Debug.LogWarning($"[SkillDB] Skill {kv.Key} prerequisite missing: {pre}");
			}
		}

		return dict;
	}
}
