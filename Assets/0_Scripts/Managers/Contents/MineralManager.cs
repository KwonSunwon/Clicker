using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;


[Serializable]
public enum MineralType
{
	Gold,
	Stone,
	Coal,
	MaxNum
	// 필요시 계속 추가 가능
}



[Serializable]
public class MineralSlot
{
	public MineralType Type;
	public BigNumber Amount;
	public BigNumber PerTick;
	public TextMeshProUGUI Text; // UI 연결 (없어도 동작)

	public MineralSlot(MineralType type, BigNumber startAmount, BigNumber startPerTick, TextMeshProUGUI text = null)
	{
		Type = type;
		Amount = startAmount;
		PerTick = startPerTick;
		Text = text;
	}
}


public class MineralManager
{

	private readonly Dictionary<MineralType, MineralSlot> _map = new Dictionary<MineralType, MineralSlot>();

	//Test 코드(엄장헌) 1.0f로 바꿀예정
	private float tickInterval = 0.01f;



	public void Init()
	{
		//Todo 세이브 데이터에서 값을 불러와서 연동할 예정
		var slots = new List<MineralSlot>
		{
			new MineralSlot(MineralType.Gold, new BigNumber(200), new BigNumber(100)),
			new MineralSlot(MineralType.Stone, new BigNumber(100), new BigNumber(300)),
			new MineralSlot(MineralType.Coal, new BigNumber(300), new BigNumber(5000))
		};

		_map.Clear();
		foreach (var slot in slots)
		{
			_map[slot.Type] = slot;
			UpdateUIText(slot, 2);
		}

		// Tick 시작
		CoroutineRunner.Instance.StartCoroutine(TickLoop());
	}


	public IEnumerator TickLoop()
	{
		var wait = new WaitForSeconds(tickInterval);
		while (true)
		{
			yield return wait;
			OnTick();
		}
	}

	private void OnTick()
    {
        foreach (var slot in _map.Values)
        {
            slot.Amount += slot.PerTick;
            UpdateUIText(slot, 2);
        }
    }
	public void UpdateUIText(MineralSlot slot, int decimals)
	{
		if (slot.Text != null)
			slot.Text.SetBigNumber(slot.Amount, decimals);
	}


	#region Public API
	public BigNumber GetAmount(MineralType type) => _map[type].Amount;

	public void Add(MineralType type, BigNumber amount)
	{
		var slot = _map[type];
		slot.Amount += amount;
		UpdateUIText(slot, 2);
	}

	public bool Spend(MineralType type, BigNumber cost)
	{
		var slot = _map[type];
		if (slot.Amount.CompareTo(cost) < 0) return false;

		slot.Amount -= cost;
		UpdateUIText(slot, 2);
		return true;
	}

	public bool CanAfford(MineralType type, BigNumber cost)
	{
		return _map[type].Amount.CompareTo(cost) >= 0;
	}

	public BigNumber GetPerTick(MineralType type) => _map[type].PerTick;

	public void SetPerTick(MineralType type, BigNumber perTick) => _map[type].PerTick = perTick;

	public void ModifyPerTick(MineralType type, BigNumber delta) => _map[type].PerTick += delta;

	public bool SpendBundle(params (MineralType type, BigNumber cost)[] costs)
	{
		foreach (var (type, cost) in costs)
		{
			if (!CanAfford(type, cost)) return false;
		}
		foreach (var (type, cost) in costs)
		{
			Spend(type, cost);
		}
		return true;
	}

	public MineralSlot GetSlot(MineralType type)
	{
		return _map[type];
	}
	#endregion

}
