using UnityEngine;
using UnityEngine.UI;

public class UI_MineRockButton : UI_MineButtonBase
{
    //NOTE: 임시로 색상 제작, 이미지 생기면 교체
    private Color normalColor = new Color(0f, 0f, 0f, 1f);
    float tempColor = 0;

    private Rock _rock;
    public Rock Rock { get { return _rock; } }

    private UI_MiningLine _line;
    public UI_MiningLine Line {
        get { return _line; }
        set { _line = value; }
    }

    private BoxCollider2D _boxCollider;

    public override void Init()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        _boxCollider.enabled = false;

        GetComponent<Image>().color = normalColor;
        _rock = GetComponent<Rock>();
    }

    public void SetTopLine()
    {
        _boxCollider.enabled = true;
    }

    protected override void HandlePointerClick()
    {
        //DISCUSS: 기본으로 콜라이더를 비활성화 하고 Top라인이 되면 그때 활성화 하는 방식으로?
        //if (Line.IsTopLine == false) {
        //    Debug.Log($"@UI_MineButton{gameObject.GetInstanceID()} Clicked but not Top Line");
        //    return;
        //}

        //NOTE: 임시로 캘 때 색상이 변경되도록함
        tempColor += 0.01f;
        GetComponent<Image>().color = new Color(normalColor.r, normalColor.g + tempColor, normalColor.b, 1f);
        Debug.Log($"@UI_MineButton{gameObject.GetInstanceID()} Clicked\nColor: {GetComponent<Image>().color}");

        if (!Rock)
            _rock = gameObject.GetOrAddComponent<Rock>();
        Rock.OnClick();

        if (Rock.IsBroken)
            Line.RockCount--;
    }
}
