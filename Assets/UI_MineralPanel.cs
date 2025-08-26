using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MineralPanel : UI_Base
{
    enum Texts
    {
		MineralAmountText
	}
    enum Images
    {
		MineralImage
	}
	public override void Init()
	{
        Bind<Image>(typeof(Images));
	}

	void Start()
    {
        
    }

    void Update()
    {
        
    }

    public TextMeshProUGUI GetMineralText()
    {
        if (GetTMP((int)Texts.MineralAmountText) == null) { 
			Bind<TextMeshProUGUI>(typeof(Texts));
		}
        return GetTMP((int)Texts.MineralAmountText);
    }
}
