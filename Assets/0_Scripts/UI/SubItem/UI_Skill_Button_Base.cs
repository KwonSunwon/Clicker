using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Skill_Button_Base : UI_Base
{
	[SerializeField] int _id;
	SkillNodeData _skillData;

	enum Texts
	{
		UI_Skill_Name_Text,
		UI_Skill_Explain_Text,
		UI_Skill_Level_Text
	}
	enum Images
	{
		UI_NodeUp,
		UI_NodeDown,
		UI_NodeLeft,
		UI_NodeRight,

		UI_Skill_Image,

	}
	enum Buttons
	{
		//UI_Skill_Button_Base
		UI_Skill_Purchase_Button
	}

	enum GameObjects
	{
		UI_Skill_Explain_Panel,
		UI_Cost_Bundle
	}
	public override void Init()
	{
		Bind<TextMeshProUGUI>(typeof(Texts));
		Bind<Image>(typeof(Images));
		Bind<Button>(typeof(Buttons));
		Bind<GameObject>(typeof(GameObjects));

		SkillManager skillManager = Managers.Skill;
		skillManager.UpdateSkillUI += UpdateButtonState;
		UI_Scene_Skill.GlobalClick += UpdatePanel;

		//ButtonID를 이용한 Init
		_skillData = skillManager.GetSkill(_id);
		Get<TextMeshProUGUI>((int)Texts.UI_Skill_Name_Text).text = _skillData.Name;
		Get<TextMeshProUGUI>((int)Texts.UI_Skill_Explain_Text).text = _skillData.Description;
		Get<TextMeshProUGUI>((int)Texts.UI_Skill_Level_Text).text = $"{_skillData.Level}/{_skillData.MaxLevel}";

		GameObject costBundle = Get<GameObject>((int)GameObjects.UI_Cost_Bundle);
		foreach(var skillCost in _skillData.SkillCost)
		{

			GameObject go = Managers.UI.MakeSubItem<UI_Cost_Image_Pack>(costBundle.transform).gameObject;
			UI_Cost_Image_Pack ImageCost = go.GetOrAddComponent<UI_Cost_Image_Pack>();
			ImageCost.SetCost(skillCost.cost);
		}

		gameObject.BindEvent(ClickedButton);
		Get<Button>((int)Buttons.UI_Skill_Purchase_Button).gameObject.BindEvent(ClickedPurchaseButton);


		Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel).SetActive(false);

		UpdateButtonState();
	}


	private void UpdateButtonState()
	{
		gameObject.SetActive(Managers.Skill.ArePrerequisitesMet(_skillData));
	
	}

	public void ClickedButton(PointerEventData data)
	{
		bool isActive = Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel).activeSelf;
		Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel).SetActive(!isActive);
		UI_Scene_Skill.GrobalClickEvent(data);
	}

	public void ClickedPurchaseButton(PointerEventData data)
	{
		bool success = Managers.Skill.TryPurchaseSkill(_id);

	}

	public void UpdatePanel(PointerEventData data)
	{
		if (!Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel).activeSelf) return;
		Debug.Log(data.pointerClick.name);
		if (data.pointerClick.gameObject!=gameObject)
		{
			Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel).SetActive(false);
		}
	}
}
