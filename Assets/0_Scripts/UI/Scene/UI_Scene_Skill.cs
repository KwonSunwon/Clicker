using DG.Tweening;
using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Scene_Skill : UI_Scene
{
	static public event Action<PointerEventData> GlobalClick;
	enum GameObjects
	{
		UI_Main_Scroll_Viewport_Content,
		UI_Main_Scroll_Viewport,
		UI_Setting_Panel

	}
	
	enum Buttons
	{
		UI_Setting_Button,
		UI_Setting_Back_Button
	}

	public override void Init()
	{
		base.Init();
		Bind<GameObject>(typeof(GameObjects));
		Bind<Button>(typeof(Buttons));
		//Get<GameObject>((int)GameObjects.UI_Main_Scroll_Viewport_Content).BindEvent(ScrollEvent,Define.UIEvent.Scroll);
		Get<GameObject>((int)GameObjects.UI_Main_Scroll_Viewport).BindEvent(ScrollEvent,Define.UIEvent.Scroll);
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
	//private float lerpSpeed = 10f; // 보간 속도
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
		var panel = Get<GameObject>((int)GameObjects.UI_Setting_Panel);

		bool isActive = panel.activeSelf;

		if (!isActive) // 켤 때
		{
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

		//UI_Scene_Skill.GrobalClickEvent(eventData);

	}

	public void CloseSettingButton(PointerEventData eventData)
	{

		if (!Get<GameObject>((int)GameObjects.UI_Setting_Panel).activeSelf) return;
		//Debug.Log(data.pointerClick.name);
		if (eventData.pointerClick.gameObject != gameObject)
		{
			var panel = Get<GameObject>((int)GameObjects.UI_Setting_Panel);
			panel.transform.DOScale(Vector3.zero, 0.5f)
				.SetEase(Ease.InBack)
				.OnComplete(() => panel.SetActive(false));
			//Get<GameObject>((int)GameObjects.UI_Skill_Explain_Panel).SetActive(false);
		}


	}
}
