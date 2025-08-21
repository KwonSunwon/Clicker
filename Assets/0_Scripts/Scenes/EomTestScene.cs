using System.Collections;
using UnityEngine;

public class EomTestScene : BaseScene
{
	protected override void Init()
	{
		base.Init();
		GoldManager goldManager = Managers.Gold;

		StartCoroutine(makeTriangle());
	}

	public override void Clear()
	{
		throw new System.NotImplementedException();
	}

	IEnumerator makeTriangle()
	{
		while (true)
		{
			var obj = Managers.Pool.Pooling_OBJ("TestTri").Get(value =>
			{
				value.transform.position = Random.insideUnitCircle*10;
				
			});

			StartCoroutine(ReturnAfterDelay("TestTri", obj, Random.Range(0.1f, 3.0f)));
			yield return new WaitForSeconds(Random.Range(0.1f,1.5f));
		}
		
	}
	private IEnumerator ReturnAfterDelay(string key, GameObject obj, float delay)
	{
		yield return new WaitForSeconds(delay);

		// obj가 이미 null이 아니고 아직 살아있다면 반환
		if (obj != null)
		{
			Managers.Pool.m_pool_Dictionary[key].Return(obj);
		}
	}
}
