using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UI_MineButtonBase : UI_Base, IPointerClickHandler
{
    protected abstract void HandlePointerClick();

    public void OnPointerClick(PointerEventData eventData)
    {
        HandlePointerClick();

        // DoTween을 사용하여 흔들림 효과 적용
        GetComponent<RectTransform>()?.DOShakeAnchorPos(
            duration: 0.1f,
            strength: new Vector2(5, 5),
            vibrato: 100,
            randomness: 180,
            snapping: false,
            fadeOut: false
        ).SetLink(gameObject);
        //NOTE: 이거 생각해보니까 광맥마다 다른 이펙트, 애니메이션을 적용해야될 수도 있는 거잖아
        // 다른 방식을 생각해봐야할거 같다
        //FIXME: 이 상태로 사용하면 TopLine이 아닌 Line에 Rock을 클릭할 때 애니메이션 동작함
    }
}
