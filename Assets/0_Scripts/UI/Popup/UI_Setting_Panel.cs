using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Setting_Panel : UI_Base
{
	private Tween _scaleTween;
	enum Buttons
	{
		UI_Setting_Back_Button
	}
	public override void Init()
	{
		{
			if (_scaleTween != null && _scaleTween.IsActive() && _scaleTween.IsPlaying())
				return;

			transform.localScale = Vector3.zero;
			// 1초 동안 0 -> 1 스케일 업
			_scaleTween = transform.DOScale(Vector3.one, 1f)
				.SetEase(Ease.OutBack).OnComplete(() => {
					_scaleTween = null;
				});

		}

		Bind<Button>(typeof(Buttons));
		Get<Button>((int)Buttons.UI_Setting_Back_Button).gameObject.BindEvent(Close, Define.UIEvent.Click);
	}

	
	public void Close(PointerEventData eventData = null)
	{
		if (_scaleTween != null && _scaleTween.IsActive() && _scaleTween.IsPlaying())
			return;

		_scaleTween = transform.DOScale(Vector3.zero, 0.5f)
				.SetEase(Ease.InBack).OnComplete(() => { 
					Managers.Resource.Destroy(gameObject);
					_scaleTween = null;
				});
	}
}
