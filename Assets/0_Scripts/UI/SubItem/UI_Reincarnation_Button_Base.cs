using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Reincarnation_Button_Base : UI_Base
{
	[SerializeField] int _id;
    ReincarnationNodeData _data;



	public override void Init()
	{
		gameObject.BindEvent(ClickedButton);
		_data = Managers.Skill.GetReincarnation(_id);
	}

	public void ClickedButton(PointerEventData data)
	{
		UI_Scene_Skill.GrobalClickEvent(data);
		UI_Scene_Reincarnation.Instance.SetReincarnationExplainPanel(_data);
	}
}
