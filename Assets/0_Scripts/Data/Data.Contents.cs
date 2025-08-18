using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{ 
#region Stat
	[Serializable]
	public class PlayerStat
	{
		public float WalkSpeed;
		public float RunSpeed;
		public float MaxStamina;
		public float RegenStaminaPerSecond;
		public float AdditionalStaminaPerCookie;
		public float NeedJumpStamina;
		public float NeedRunStaminaPerSec;

		//플레이어 소리범위설정용
		public float WalkNoiseRange = 30f;
        public float RunNoiseRange = 50f;
        public float LandNoiseRange = 100f;
    }

	[Serializable]
	public class GameSetting
	{
		public float CameraMaxPitch;
		public float CameraMinPitch;
		public float InteractionDistance;
		public float MouseSensitivity;
		
	}

	//[Serializable]
	//public class StatData : ILoader<int, Stat>
	//{
	//	public List<Stat> stats = new List<Stat>();

	//	public Dictionary<int, Stat> MakeDict()
	//	{
	//		Dictionary<int, Stat> dict = new Dictionary<int, Stat>();
	//		foreach (Stat stat in stats)
	//			dict.Add(stat.level, stat);
	//		return dict;
	//	}
	//}
	#endregion


	[Serializable]
	public class Dialog
	{
		public string name;
		public string text;
	}

	[Serializable]
	public class DialogData : ILoader<string, string>
	{
		public List<Dialog> dialogs = new List<Dialog>();

		public Dictionary<string, string> MakeDict()
		{
			Dictionary<string, string> dict = new Dictionary<string, string>();
			foreach (Dialog dialog in dialogs)
				dict.Add(dialog.name, dialog.text);
			return dict;
		}
	}

}