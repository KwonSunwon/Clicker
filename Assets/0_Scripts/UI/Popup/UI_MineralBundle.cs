using UnityEngine;

public class UI_MineralBundle : UI_Base
{
	public override void Init()
	{
		for (int i = 0; i < (int)MineralType.MaxNum; i++) 
		{
			UI_MineralPanel mineralPanel = Managers.UI.MakeSubItem<UI_MineralPanel>(transform);
			MineralSlot slot = Managers.Mineral.GetSlot((MineralType)i);
			if (slot == null) { Managers.Resource.Destroy(mineralPanel.gameObject);  return; }
			slot.Text = mineralPanel.GetMineralText();
			Managers.Mineral.UpdateUIText(slot, 2);
		}
	}

	void Start()
    {
		Init();

	}


}
