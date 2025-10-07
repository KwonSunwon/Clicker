using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class VeinView : MonoBehaviour, IPointerClickHandler
{
    public int Id { get; private set; }

    Action<int> OnClick;

    public void Bind(VeinState data, RockView rock, Action<int> onClick)
    {
        Id = MineDomain.GetDec(data.Id);
        OnClick = onClick;

        SetPosition(rock);
    }

    public void SetPosition(RockView rock)
    {
        transform.position = Vector3.zero;
        transform.localScale = Vector3.one;
        var rockRT = rock.GetComponent<RectTransform>();
        var oreRT = GetComponent<RectTransform>();
        oreRT.sizeDelta = rockRT.sizeDelta;
        oreRT.anchoredPosition = rockRT.anchoredPosition;

        GetComponent<UIColliderSizeSync>().SetSize();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick?.Invoke(Id);
    }

    public void PlayDoTween()
    {
        // DoTween
        GetComponent<RectTransform>()?.DOShakeAnchorPos(
            duration: 0.1f,
            strength: new Vector2(5, 5),
            vibrato: 100,
            randomness: 180,
            snapping: false,
            fadeOut: false
        ).SetLink(gameObject);
    }
}
