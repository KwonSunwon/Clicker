using DG.Tweening;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UI_Skill_Button_Base : UI_Base
{
	[SerializeField] int _id;
	SkillNodeData _skillData;
	public void SetId(int id)
	{
		_id = id;
	}

	enum Texts
	{
		UI_Skill_Name_Text,
		UI_Skill_Explain_Text,
		UI_Skill_Level_Text
	}
	enum Images
	{
		UI_NodeUp,
		UI_NodeRightUp,
		UI_NodeRight,
		UI_NodeRightDown,
		UI_NodeDown,
		UI_NodeLeftDown,
		UI_NodeLeft,
		UI_NodeLeftUp,

		UI_Skill_Image,
		UI_Skill_Tree_Image

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

		Get<Image>((int)Images.UI_Skill_Image).sprite = Resources.Load<Sprite>($"Art/SkillTree/Skilltree_{_id}");
		Get<Image>((int)Images.UI_Skill_Tree_Image).sprite = Resources.Load<Sprite>($"Art/SkillTree/Skilltree_{_id}");

		RectTransform rect = GetComponent<RectTransform>();
		Vector3 pos = rect.localPosition;
		pos.x = _skillData.Xpos;
		pos.y = _skillData.Ypos;
		rect.localPosition = pos;

		List<Image> _nodeImages = new List<Image>
		{
			Get<Image>((int)Images.UI_NodeUp),
			Get<Image>((int)Images.UI_NodeRightUp),
			Get<Image>((int)Images.UI_NodeRight),
			Get<Image>((int)Images.UI_NodeRightDown),
			Get<Image>((int)Images.UI_NodeDown),
			Get<Image>((int)Images.UI_NodeLeftDown),
			Get<Image>((int)Images.UI_NodeLeft),
			Get<Image>((int)Images.UI_NodeLeftUp)
		};
		for (int i = 0; i < _nodeImages.Count; i++)
		{
			if (_nodeImages[i] != null)
				_nodeImages[i].gameObject.SetActive(false);
		}
		foreach (int idx in _skillData.Edges)
		{
			if (idx < 0 || idx >= _nodeImages.Count)
			{
				Debug.LogWarning($"[UI_Skill_Button_Base] Edge idx {idx} out of range");
				continue;
			}

			var img = _nodeImages[idx];
			if (img != null)
				img.gameObject.SetActive(true);
		}


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
		if (_skillData.Level == 0)
		{
			GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/UI_SkillTree/Rect_Disabled");
		}
		else
		{
			GetComponent<Image>().sprite = Resources.Load<Sprite>("Art/UI_SkillTree/Rect_Normal");
		}


	}

	public void ClickedButton(PointerEventData data)
	{
		var panel = Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel);

		bool isActive = panel.activeSelf;
		
		if (!isActive) // 켤 때
		{
			transform.SetAsLastSibling();
			panel.SetActive(true);
			// DOTween 초기화
			panel.transform.localScale = Vector3.zero;

			// 1초 동안 0 -> 1 스케일 업
			panel.transform.DOScale(Vector3.one, 1f)
				.SetEase(Ease.OutBack); // 부드럽게 튀어나오는 느낌
		}
		else // 끌 때
		{
			// 꺼질 때 애니메이션도 넣고 싶으면 여기
			panel.transform.DOScale(Vector3.zero, 0.5f)
				.SetEase(Ease.InBack)
				.OnComplete(() => panel.SetActive(false));
		}

		UI_Scene_Skill.GrobalClickEvent(data);
	}

	public void ClickedPurchaseButton(PointerEventData data)
	{
		bool success = Managers.Skill.TryPurchaseSkill(_id);

	}

	public void UpdatePanel(PointerEventData data)
	{
		if (!Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel).activeSelf) return;
		//Debug.Log(data.pointerClick.name);
		if (data.pointerClick.gameObject!=gameObject)
		{
			var panel = Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel);
			panel.transform.DOScale(Vector3.zero, 0.5f)
				.SetEase(Ease.InBack)
				.OnComplete(() => panel.SetActive(false));
			//Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel).SetActive(false);
		}
	}
}
