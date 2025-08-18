using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class GoldManager
{
	[SerializeField] private TMP_Text goldText;

	private BigNumber gold;
	private BigNumber goldPerTick;

	//Test 코드(엄장헌) 1.0f로 바꿀예정
	private float tickInterval = 0.01f;

	public void Init()
	{
		//Todo 이전 골드 데이터 불러오기 (순원)


		//Test코드 나중에는 골드데이터 불러오기
		gold = new BigNumber(0);

		//Test코드 나중에는 해금된 내역과 아이템에 따라 계산할에정
		//CalculateGoldPerTick();
		goldPerTick = new BigNumber(100000);

		Transform root = Managers.UI.Root.transform;
		Transform goldObj = root.Find("UI_GoldText");
		if (goldObj == null)
		{
			// 없으면 UI Popup 생성
			var popup = Managers.UI.ShowPopupUI<UI_GoldText>();
			goldText = popup.GetComponentInChildren<TMP_Text>();
		}
		else
		{
			goldText = goldObj.GetComponent<TMP_Text>();
		}

		// 시작 값 세팅
		goldText.SetBigNumber(gold);

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
		AddGold(goldPerTick);


		//Test 코드(엄장헌) 지울예정
		goldPerTick = goldPerTick * 1.1;
	}

	/// <summary>
	/// 골드 추가
	/// </summary>
	public void AddGold(BigNumber amount)
	{
		gold += amount;
		goldText.SetBigNumber(gold, decimals: 2);
		Debug.Log($"현재 골드: {gold}");
	}

	/// <summary>
	/// 골드 차감
	/// </summary>
	public void SpendGold(BigNumber cost)
	{
		if (gold.CompareTo(cost) >= 0)
		{
			gold -= cost;
			goldText.SetBigNumber(gold, decimals: 2);
			Debug.Log($"소모 후 골드: {gold}");
		}
		else
		{
			Debug.Log("골드 부족!");
		}
	}

}
