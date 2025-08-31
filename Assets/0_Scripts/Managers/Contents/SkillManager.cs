using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager
{
	private Dictionary<int, SkillNodeData> SkillMap = new Dictionary<int, SkillNodeData>();
	public event Action UpdateSkillUI;
	public void Init()
	{
		//SkillNodeData 중 level을 제외한 모든 파라미터는 엑셀 파일로 받아온다.
		//SkillNodeData 중 현재 level을 로컬 파일에서 받아온다.
		//Todo 원짱 해줘

		Debug.Log("Skill Init");

		SkillNodeData skill1 = new SkillNodeData();
		skill1.Id = 1;
		skill1.Name = "더블 점프";
		skill1.Level = 0;
		skill1.Description = "공중에서 한 번 더 점프할 수 있습니다.";
		skill1.SkillCost = new List<(MineralType, BigNumber)>
		{
			(MineralType.Coal, new BigNumber(50))
		};
		skill1.precedingSkills = new List<int>();

		SkillNodeData skill2 = new SkillNodeData();
		skill2.Id = 2;
		skill2.Name = "강한 공격";
		skill2.Level = 0;
		skill2.Description = "공격력이 10% 증가합니다.";
		skill2.SkillCost = new List<(MineralType, BigNumber)>
		{
			(MineralType.Coal, new BigNumber(30)),
			(MineralType.Stone, new BigNumber(20))
		};
		skill2.precedingSkills = new List<int> { 1 };

		SkillNodeData skill3 = new SkillNodeData();
		skill3.Id = 3;
		skill3.Name = "황금 갑옷";
		skill3.Level = 0;
		skill3.Description = "방어력이 15% 증가합니다.";
				skill3.SkillCost = new List<(MineralType, BigNumber)>
		{
			(MineralType.Gold, new BigNumber(5))
		};
		skill3.precedingSkills = new List<int> { 1, 2 };

		SkillMap.Add(1, skill1);
		SkillMap.Add(2, skill2);
		SkillMap.Add(3, skill3);
	}

	//Todo 스킬 세이브 로드 기능 해줘 순원
	public SkillNodeData GetSkill(int id)
	{
		if (SkillMap.TryGetValue(id, out var skill))
		{
			return skill;
		}

		Debug.LogWarning($"Skill with Id {id} not found.");
		return null;
	}

	//Todo순원 로컬에 배운 스킬 저장
	private void SaveSkills()
	{
		UpdateSkillUI?.Invoke();
	}

	public bool ArePrerequisitesMet(SkillNodeData skill)
	{
		if (skill.precedingSkills == null || skill.precedingSkills.Count == 0)
			return true;

		foreach (var preId in skill.precedingSkills)
		{
			SkillNodeData preSkill = GetSkill(preId);
			if (preSkill == null || preSkill.Level == 0)
				return false;
		}
		return true;
	}

	public bool TryPurchaseSkill(int id)
	{
		SkillNodeData skill = GetSkill(id);
		if (skill == null)
		{
			UnityEngine.Debug.LogWarning($"Skill with Id {id} not found.");
			return false;
		}

		// 이미 배운 스킬인지 체크
		if (skill.Level > 0)
		{
			UnityEngine.Debug.LogWarning($"Skill {skill.Name} already learned.");
			return false;
		}


		// 1. 구매 가능한지 체크
		foreach (var (mineralType, cost) in skill.SkillCost)
		{
			if (Managers.Mineral.GetAmount(mineralType).CompareTo(cost) < 0)
			{
				UnityEngine.Debug.LogWarning($"{skill.Name} 구매 실패: {mineralType} 부족");
				return false; // 하나라도 부족하면 실패
			}
		}

		// 2. 실제로 자원 차감
		foreach (var (mineralType, cost) in skill.SkillCost)
		{
			Managers.Mineral.Spend(mineralType, cost);
		}

		// 3. 스킬 레벨 올리기
		skill.Level = 1; // 처음 배우는 경우 1로 설정 (혹은 += 1로 여러 레벨 가능)

		UnityEngine.Debug.Log($"{skill.Name} 스킬 구매 성공!");
		SaveSkills();
		return true;
	}

}

public class SkillNodeData
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int MaxLevel { get; set; }
	public string Description { get; set; }

	public List<(MineralType mineralType, BigNumber cost)> SkillCost { get; set; }
	public List<int> precedingSkills { get; set; }

	public int Level { get; set; } //0이라면 배우지 않은것
	

}