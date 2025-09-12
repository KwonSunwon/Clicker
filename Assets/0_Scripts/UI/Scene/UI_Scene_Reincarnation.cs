using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.UI.Scrollbar;

public class UI_Scene_Reincarnation : UI_Scene
{
	static public event Action<PointerEventData> GlobalClick;
	static public UI_Scene_Reincarnation Instance {  get; private set; }
	enum GameObjects
	{
		UI_Main_Scroll_Viewport_Content,
		UI_Main_Scroll_Viewport,
		UI_Setting_Panel,
		UI_Reincarnation_Explain_Panel

	}
	enum Buttons
	{
		UI_Setting_Button,
		UI_Setting_Back_Button
	}

	public override void Init()
	{
		if (Instance == null) { Instance = this; }

		base.Init();
		Bind<GameObject>(typeof(GameObjects));
		Bind<Button>(typeof(Buttons));
		//Get<GameObject>((int)GameObjects.UI_Main_Scroll_Viewport_Content).BindEvent(ScrollEvent,Define.UIEvent.Scroll);
		Get<GameObject>((int)GameObjects.UI_Main_Scroll_Viewport).BindEvent(ScrollEvent, Define.UIEvent.Scroll);
		Get<GameObject>((int)GameObjects.UI_Main_Scroll_Viewport).BindEvent(GrobalClickEvent, Define.UIEvent.Click);
		Get<Button>((int)Buttons.UI_Setting_Button).gameObject.BindEvent(ClickedSettingButton, Define.UIEvent.Click);
		Get<Button>((int)Buttons.UI_Setting_Back_Button).gameObject.BindEvent(ClickedSettingButton, Define.UIEvent.Click);
		Get<GameObject>((int)GameObjects.UI_Setting_Panel).SetActive(false);

		

		GlobalClick += CloseSettingButton;
	}

	private RectTransform content;
	private float zoomSpeed = 0.01f;
	private float minZoom = 0.5f;
	private float maxZoom = 2f;
	private Vector3 targetScale;
	public void ScrollEvent(PointerEventData eventData)
	{
		if (content == null)
		{
			content = Get<GameObject>((int)GameObjects.UI_Main_Scroll_Viewport_Content).GetComponent<RectTransform>();
		}

		float scroll = eventData.scrollDelta.y; // 마우스 휠 입력
		if (scroll != 0)
		{
			Vector3 scale = content.localScale;
			scale += Vector3.one * (scroll * zoomSpeed);
			scale = new Vector3(
				Mathf.Clamp(scale.x, minZoom, maxZoom),
				Mathf.Clamp(scale.y, minZoom, maxZoom),
				1f
			);
			content.localScale = scale;
		}
	}

	public static void GrobalClickEvent(PointerEventData eventData)
	{
		GlobalClick?.Invoke(eventData);
	}

	public void ClickedSettingButton(PointerEventData eventData)
	{
		var settingPanel = Get<GameObject>((int)GameObjects.UI_Setting_Panel);
		settingPanel.SetActive(!settingPanel.activeSelf);

	}
	public void CloseSettingButton(PointerEventData eventData)
	{
		Get<GameObject>((int)GameObjects.UI_Setting_Panel).SetActive(false);
	}

	public void SetReincarnationExplainPanel(ReincarnationNodeData reincarnationNodeData)
	{
		Get<GameObject>((int)GameObjects.UI_Reincarnation_Explain_Panel).GetComponent<UI_Reincarnation_Explain_Panel>().SetText(reincarnationNodeData);
	}



}
