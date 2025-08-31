using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Cost_Image_Pack : UI_Base
{
	enum Images
	{
		UI_Skill_Cost_Image
	}

	enum Texts
	{
		UI_Skill_Cost_Text
	}
	public override void Init()
	{
		Bind<Image>(typeof(Images));
	}

	public void SetCost(BigNumber cost)
	{
		Bind<TextMeshProUGUI>(typeof(Texts));

		Get<TextMeshProUGUI>((int)Texts.UI_Skill_Cost_Text).text = cost.ToString();
	}


}
