using System.Collections.Generic;
using UnityEngine;

public class UI_Skill_Button_Parent : MonoBehaviour
{
	[SerializeField] private string _prefabPath = "Prefabs/UI/SubItem/UI_Skill_Button_Base";
	private void Start()
	{
		InitSkillNodes(Managers.Skill.SkillMap);
	}
	public void InitSkillNodes(Dictionary<int, SkillNodeData> skillMap)
    {
		GameObject prefab = Resources.Load<GameObject>(_prefabPath);
		foreach (var kvp in skillMap)
		{
			int id = kvp.Key;

			// 생성
			GameObject go = Instantiate(prefab, transform);

			// 초기화
			UI_Skill_Button_Base button = go.GetComponent<UI_Skill_Button_Base>();
			if (button != null)
				button.SetId(id);

		}
	}

}
