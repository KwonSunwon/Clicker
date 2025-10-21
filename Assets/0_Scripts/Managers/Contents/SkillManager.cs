using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillManager
{
	public Dictionary<int, SkillNodeData> SkillMap = new Dictionary<int, SkillNodeData>();
	private Dictionary<int, ReincarnationNodeData> ReincarnationMap = new Dictionary<int, ReincarnationNodeData>();
	public event Action UpdateSkillUI;
	public void Init()
	{
		SkillMap = DataManager.BuildFromResources("Data/SkillData");
		//SkillNodeData 중 level을 제외한 모든 파라미터는 엑셀 파일로 받아온다.
		//SkillNodeData 중 현재 level을 로컬 파일에서 받아온다.
		//Todo: 일단 임시로 15개 해두는데 전부 파일에서 읽어와야 함

		{
			Debug.Log("Rein Init");

			ReincarnationNodeData skill1 = new ReincarnationNodeData();
			skill1.Id = 1;
			skill1.Name = "더블 점프";
			skill1.Level = 0;
			skill1.Description = "공중에서 한 번 더 점프할 수 있습니다.";
			skill1.SkillCost = 1;
			skill1.precedingSkills = new List<int>();

			ReincarnationNodeData skill2 = new ReincarnationNodeData();
			skill2.Id = 2;
			skill2.Name = "강한 공격";
			skill2.Level = 0;
			skill2.Description = "공격력이 10% 증가합니다.";
			skill2.SkillCost = 10;
			skill2.precedingSkills = new List<int> { 1 };

			ReincarnationNodeData skill3 = new ReincarnationNodeData();
			skill3.Id = 3;
			skill3.Name = "황금 갑옷";
			skill3.Level = 0;
			skill3.Description = "방어력이 15% 증가합니다.";
			skill3.SkillCost = 50;
			skill3.precedingSkills = new List<int> { 1, 2 };


			ReincarnationNodeData skill4 = new ReincarnationNodeData();
			skill4.Id = 4;
			skill4.Name = "김재경 빨리하라고";
			skill4.Level = 0;
			skill4.Description = "빨리하라고";
			skill4.SkillCost = 100;
			skill4.precedingSkills = new List<int> { 1, 2 };

			ReincarnationMap.Add(1, skill1);
			ReincarnationMap.Add(2, skill2);
			ReincarnationMap.Add(3, skill3);
			ReincarnationMap.Add(4, skill4);

		}
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

	public ReincarnationNodeData GetReincarnation(int id)
	{
		if(ReincarnationMap.TryGetValue(id,out var reincarnation))
		{
			return reincarnation;
		}

		Debug.LogWarning($"reincarnation with Id {id} not found.");
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

	public bool ArePrerequisitesMet(ReincarnationNodeData reincarnation)
	{
		if (reincarnation.precedingSkills == null || reincarnation.precedingSkills.Count == 0)
			return true;

		foreach (var preId in reincarnation.precedingSkills)
		{
			ReincarnationNodeData preSkill = GetReincarnation(preId);
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

	public bool TryPurchaseReincarnation(int id)
	{
		ReincarnationNodeData reincarnation = GetReincarnation(id);
		if (reincarnation == null)
		{
			UnityEngine.Debug.LogWarning($"reincarnation with Id {id} not found.");
			return false;
		}

		// 이미 배운 스킬인지 체크
		if (reincarnation.Level > 0)
		{
			UnityEngine.Debug.LogWarning($"Skill {reincarnation.Name} already learned.");
			return false;
		}


		// 1. 구매 가능한지 체크
		if (Managers.Mineral.ReincarnationCoin - reincarnation.SkillCost < 0)
		{
			UnityEngine.Debug.LogWarning($"{reincarnation.Name} 구매 실패: 코인 부족");
			return false; // 하나라도 부족하면 실패
		}
		

		// 2. 실제로 자원 차감

		Managers.Mineral.Spend(reincarnation.SkillCost);


		// 3. 스킬 레벨 올리기
		reincarnation.Level = 1; // 처음 배우는 경우 1로 설정 (혹은 += 1로 여러 레벨 가능)

		UnityEngine.Debug.Log($"{reincarnation.Name} 스킬 구매 성공!");
		SaveSkills();
		return true;
	}




}

public class SkillNodeData
{
	public int Id { get; set; }
	public string Name { get; set; }
	public string Description { get; set; }
	public int MaxLevel { get; set; }
	

	public List<(MineralType mineralType, BigNumber cost)> SkillCost { get; set; }
	public List<int> precedingSkills { get; set; }
	public List<int> Edges { get; set; }

	public int Xpos {  get; set; }
	public int Ypos { get; set; }
	public int Level { get; set; } //0이라면 배우지 않은것




}

public class ReincarnationNodeData
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int MaxLevel { get; set; }
	public string Description { get; set; }

	public int SkillCost { get; set; }
	public List<int> precedingSkills { get; set; }

	public int Level { get; set; } //0이라면 배우지 않은것
}