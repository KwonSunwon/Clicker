using Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Define;

public class SettingManager
{

	public float UISound { get; private set; }
	public float InGameSound { get; private set; }
	public float BgmSound { get; private set; }
	public float Gamma { get; private set; }
	public float MouseSense { get; private set; }


	public KeyCode FrontKey { get; private set; }
	public KeyCode BackKey { get; private set; }
	public KeyCode LeftKey { get; private set; }
	public KeyCode RightKey { get; private set; }
	public KeyCode JumpKey { get; private set; }
	public KeyCode RunKey { get; private set; }
	public KeyCode InteractionKey { get; private set; }
	public KeyCode Inventory1Key { get; private set; }
	public KeyCode Inventory2Key { get; private set; }
	public KeyCode Inventory3Key { get; private set; }
	public KeyCode Inventory4Key { get; private set; }
	public KeyCode Inventory5Key { get; private set; }


	public void Load()
	{
		UISound = PlayerPrefs.GetFloat("UISound", 1.0f);
		InGameSound = PlayerPrefs.GetFloat("InGameSound", 0.8f);
		BgmSound = PlayerPrefs.GetFloat("BgmSound", 1.0f);
		Gamma = PlayerPrefs.GetFloat("Gamma", 1.0f);
		MouseSense = PlayerPrefs.GetFloat("MouseSense", 1.0f);

		FrontKey = LoadKey("FrontKey", KeyCode.W);
		BackKey = LoadKey("BackKey", KeyCode.S);
		LeftKey = LoadKey("LeftKey", KeyCode.A);
		RightKey = LoadKey("RightKey", KeyCode.D);
		InteractionKey = LoadKey("InteractionKey", KeyCode.E);
		JumpKey = LoadKey("JumpKey", KeyCode.Space);
		RunKey = LoadKey("RunKey", KeyCode.LeftShift);

		Inventory1Key = LoadKey("Inventory1Key", KeyCode.Alpha1);
		Inventory2Key = LoadKey("Inventory2Key", KeyCode.Alpha2);
		Inventory3Key = LoadKey("Inventory3Key", KeyCode.Alpha3);
		Inventory4Key = LoadKey("Inventory4Key", KeyCode.Alpha4);
		Inventory5Key = LoadKey("Inventory5Key", KeyCode.Alpha5);
	}

	public void Save()
	{
		PlayerPrefs.SetFloat("UISound", UISound);
		PlayerPrefs.SetFloat("InGameSound", InGameSound);
		PlayerPrefs.SetFloat("BgmSound", BgmSound);
		PlayerPrefs.SetFloat("Gamma", Gamma);
		PlayerPrefs.SetFloat("MouseSense", MouseSense);

		SaveKey("FrontKey", FrontKey);
		SaveKey("BackKey", BackKey);
		SaveKey("LeftKey", LeftKey);
		SaveKey("RightKey", RightKey);
		SaveKey("JumpKey", JumpKey);
		SaveKey("RunKey", RunKey);


		SaveKey("InteractionKey", InteractionKey);

		SaveKey("Inventory1Key", Inventory1Key);
		SaveKey("Inventory2Key", Inventory2Key);
		SaveKey("Inventory3Key", Inventory3Key);
		SaveKey("Inventory4Key", Inventory4Key);
		SaveKey("Inventory5Key", Inventory5Key);

		PlayerPrefs.Save();
	}

	// 개별 값 설정용 (외부 UI에서 호출)
	public void SetUISound(float value) => UISound = value;
	public void SetInGameSound(float value) => InGameSound = value;
	public void SetBgmSound(float value) => BgmSound = value;
	public void SetGamma(float value) => Gamma = value;
	public void SetMouseSense(float value) => MouseSense = value;

	public void SetKey(string keyName, KeyCode keyCode)
	{
		switch (keyName)
		{
			case "FrontKey": FrontKey = keyCode; break;
			case "BackKey": BackKey = keyCode; break;
			case "LeftKey": LeftKey = keyCode; break;
			case "RightKey": RightKey = keyCode; break;
			case "JumpKey": JumpKey = keyCode; break;
			case "RunKey": RunKey = keyCode; break;
			case "InteractionKey": InteractionKey = keyCode; break;
			case "Inventory1Key": Inventory1Key = keyCode; break;
			case "Inventory2Key": Inventory2Key = keyCode; break;
			case "Inventory3Key": Inventory3Key = keyCode; break;
			case "Inventory4Key": Inventory4Key = keyCode; break;
			case "Inventory5Key": Inventory5Key = keyCode; break;
			default: Debug.LogWarning($"Unknown key: {keyName}"); break;
		}
	}

	// 내부 유틸리티
	private KeyCode LoadKey(string key, KeyCode defaultKey)
	{
		string saved = PlayerPrefs.GetString(key, defaultKey.ToString());
		return Enum.TryParse(saved, out KeyCode result) ? result : defaultKey;
	}

	private void SaveKey(string key, KeyCode code)
	{

		PlayerPrefs.SetString(key, code.ToString());
	}

	public void ApplySettings(List<(string keyName, float value)> soundList, List<(string keyName, KeyCode key)> keyList)
	{
		foreach (var (keyName, value) in soundList)
		{
			switch (keyName)
			{
				case "UISound": SetUISound(value); break;
				case "InGameSound": SetInGameSound(value); break;
				case "BgmSound": SetBgmSound(value); break;
				case "Gamma": SetGamma(value); break;
				case "MouseSense": SetMouseSense(value); break;
				default:
					Debug.LogWarning($"Unknown sound setting key: {keyName}");
					break;
			}
		}

		foreach (var (keyName, keyCode) in keyList)
		{
			SetKey(keyName, keyCode); // 이미 메서드 있음
		}
	}
}