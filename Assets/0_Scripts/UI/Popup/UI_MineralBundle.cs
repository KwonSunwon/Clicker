using UnityEngine;

public class UI_MineralBundle : UI_Base
{
	public override void Init()
	{
		//Todo: 
		for (int i = 0; i < /*(int)MineralType.MaxNum*/14; i++) 
		{
			UI_MineralPanel mineralPanel = Managers.UI.MakeSubItem<UI_MineralPanel>(transform);
			MineralSlot slot = Managers.Mineral.GetSlot((MineralType)i);
			if (slot == null) { Managers.Resource.Destroy(mineralPanel.gameObject);  return; }
			slot.Text = mineralPanel.GetMineralText();
			mineralPanel.SetMineralImage(slot.Sprite);


			Managers.Mineral.UpdateUIText(slot, 2);
		}
	}

	void Start()
    {
		Init();

	}


}
