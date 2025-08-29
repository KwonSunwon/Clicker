using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_PointerClickEventHandler : MonoBehaviour, IPointerClickHandler
{
	public Action<PointerEventData> OnClickHandler = null;


	public void OnPointerClick(PointerEventData eventData)
	{
		if (OnClickHandler != null)
			OnClickHandler.Invoke(eventData);
	}


}

public class UI_IDragEventHandler : MonoBehaviour, IDragHandler
{
	public Action<PointerEventData> OnDragHandler = null;
	public void OnDrag(PointerEventData eventData)
	{
		if (OnDragHandler != null)
			OnDragHandler.Invoke(eventData);
	}
}

public class UI_PointerEnterEventHandler : MonoBehaviour, IPointerEnterHandler
{
	public Action<PointerEventData> OnEnterHandler = null;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (OnEnterHandler != null)
			OnEnterHandler.Invoke(eventData);
	}
}

public class UI_PointerExitEventHandler : MonoBehaviour, IPointerExitHandler
{
	public Action<PointerEventData> OnExitHandler = null;

	public void OnPointerExit(PointerEventData eventData)
	{
		if (OnExitHandler != null)
			OnExitHandler.Invoke(eventData);
	}
}

public class UI_ScrollEventHandler : MonoBehaviour, IScrollHandler
{

	public Action<PointerEventData> OnScrollHandler = null;


	public void OnScroll(PointerEventData eventData)
	{
		if (OnScrollHandler != null)
			OnScrollHandler.Invoke(eventData);
	}
}
