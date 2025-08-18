using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inven_Item : UI_Base
{
    enum GameObjects
    {
        ItemIcon,
        ItemNameText,
    }

    string _name;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = _name;

        Get<GameObject>((int)GameObjects.ItemIcon).BindEvent((PointerEventData) => { Debug.Log($"아이템 클릭! {_name}"); });
    }

    public void SetInfo(string name)
    {
        _name = name;
		
	}

    public void SetText(string text)
    {
		Get<GameObject>((int)GameObjects.ItemNameText).GetComponent<Text>().text = text;
	}

	public void SetImage(int type)
	{
		Image img = Get<GameObject>((int)GameObjects.ItemIcon).GetComponent<Image>();

		Sprite sprite = Managers.Resource.Load<Sprite>($"Art/Cookie{type}");
		if (sprite != null)
		{
			img.sprite = sprite;
		}
		else
		{
			Debug.LogWarning($"Sprite not found at path: Art/Cookie{type}");
		}
	}
}
