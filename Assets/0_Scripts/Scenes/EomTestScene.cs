using System.Collections;
using UnityEngine;

public class EomTestScene : BaseScene
{
	protected override void Init()
	{
		base.Init();
		MineralManager mineralManager = Managers.Mineral;
		Managers.UI.ShowSceneUI<UI_Scene_Skill>();

	}

	public override void Clear()
	{
		throw new System.NotImplementedException();
	}

}
