using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static Util;

public class RockView : MonoBehaviour, IPointerClickHandler
{
    Image _img;
    BoxCollider2D _collider;

    [SerializeField] OreSpriteSet SpriteSet;

    public int Id { get; private set; }

    Action<int> OnClick;

    public void Bind(RockState data, Action<int> onClick)
    {
        _img = GetComponent<Image>();
        _img.sprite = SpriteSet.sprites[0];

        _collider = GetComponent<BoxCollider2D>();
        _collider.enabled = true;

        Id = GetDec(data.Id);
        OnClick = onClick;
        Refresh(data);
    }

    public void Refresh(RockState data)
    {
        Debug.Log($"Rock {Id} Refreshed: Hp={data.Hp}, IsBroken={data.IsBroken}");

        if (data.Hp < 6) _img.sprite = SpriteSet.sprites[data.Hp];
        if (data.IsBroken) PlayBreak();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"Rock {Id} Clicked");

        OnClick?.Invoke(Id);
    }

    public void PlayBreak()
    {
        Debug.Log($"Rock {Id} Broken");

        _img.enabled = false;
        _collider.enabled = false;
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
