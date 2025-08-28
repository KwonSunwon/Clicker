using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Scene_Skill : UI_Scene
{
   enum GameObjects
	{
		UI_Main_Scroll_Viewport_Content,
		UI_Main_Scroll_Viewport

	}

	public override void Init()
	{
		base.Init();
		Bind<GameObject>(typeof(GameObjects));
		//Get<GameObject>((int)GameObjects.UI_Main_Scroll_Viewport_Content).BindEvent(ScrollEvent,Define.UIEvent.Scroll);
		Get<GameObject>((int)GameObjects.UI_Main_Scroll_Viewport).BindEvent(ScrollEvent,Define.UIEvent.Scroll);
	}

	private RectTransform content;
	private float zoomSpeed = 0.01f;
	private float minZoom = 0.5f;
	private float maxZoom = 2f;
	private Vector3 targetScale;
	private float lerpSpeed = 10f; // 보간 속도
	public void ScrollEvent(PointerEventData eventData)
	{
		if (eventData.pointerCurrentRaycast.gameObject != null)
		{
			Debug.Log("현재 레이캐스트 타겟: " + eventData.pointerCurrentRaycast.gameObject.name);
		}
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

	
}
