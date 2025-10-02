using System;
using UnityEngine;

public class UI_MineRockButton : UI_MineButtonBase
{
    //NOTE: 임시로 색상 제작, 이미지 생기면 교체
    private Color normalColor = new Color(0f, 0f, 0f, 1f);
    float tempColor = 0;

    private Rock _rock;
    public Rock Rock { get { return _rock; } }

    private BoxCollider2D _boxCollider;

    public event Action<UI_MineRockButton> OnMineRockBroken;

    public void Awake()
    {
        _rock ??= gameObject.GetOrAddComponent<Rock>();
        _boxCollider ??= GetComponent<BoxCollider2D>();
        _boxCollider.enabled = false;
    }

    public override void Init()
    {
        //GetComponent<Image>().color = normalColor;
        _rock ??= GetComponent<Rock>();
    }

    public void SetTopLine(bool isTop = true)
    {
        _boxCollider ??= GetComponent<BoxCollider2D>();
        _boxCollider.enabled = isTop;
    }

    protected override void HandlePointerClick()
    {
        //NOTE: 임시로 캘 때 색상이 변경되도록함
        //tempColor += 0.01f;
        //GetComponent<Image>().color = new Color(normalColor.r, normalColor.g + tempColor, normalColor.b, 1f);
        //Debug.Log($"@UI_MineButton{gameObject.GetInstanceID()} Clicked\nColor: {GetComponent<Image>().color}");

        if (!Rock)
            _rock ??= gameObject.GetOrAddComponent<Rock>();
        Rock.OnClick();

        if (Rock.IsBroken) {
            Break();
        }
    }

    public void Break()
    {
        Rock.Break();
        OnMineRockBroken?.Invoke(this);
    }
}
