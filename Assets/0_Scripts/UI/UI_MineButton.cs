using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MineButton : UI_Base
{
    OreBase oreBase;

    public override void Init()
    {
        gameObject.BindEvent(OnClickButton, Define.UIEvent.Click);

        oreBase = GetComponent<OreBase>();
        if (oreBase == null)
            Debug.LogError($"OreBase is null! {gameObject.GetInstanceID()}");
        oreBase.Init();
    }

    public void OnClickButton(PointerEventData data)
    {
        oreBase.OnClick();
        //Debug.Log("UI_MineButton Clicked2");
    }
}
