using UnityEngine;

public class UI_MineralBundle : UI_Base
{
	public override void Init()
	{
		for (int i = 0; i < (int)MineralType.MaxNum; i++) 
		{
			UI_MineralPanel mineralPanel = Managers.UI.MakeSubItem<UI_MineralPanel>(transform);
			MineralSlot slot = Managers.Mineral.GetSlot((MineralType)i);
			slot.Text = mineralPanel.GetMineralText();
			Managers.Mineral.UpdateUIText(slot, 2);
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
    {
		Init();

	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
