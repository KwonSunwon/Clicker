// BigNumberUIHelper.cs
using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public static class BigNumberUIHelper
{
	// 기본 접미사 (1000 단위: "", A, B, ..., Z, AA, AB, ...)
	private static readonly string[] DefaultSuffix = BuildDefaultSuffix();

	/// <summary>
	/// TMP 텍스트에 BigNumber를 지정 포맷으로 표시합니다.
	/// </summary>
	/// <param name="label">대상 TMP 텍스트</param>
	/// <param name="num">표시할 수</param>
	/// <param name="decimals">소수 자릿수</param>
	/// <param name="suffixes">접미사 표 (null 이면 기본)</param>
	/// <param name="fallbackScientific">접미사 범위를 초과하면 e지수 표기 사용</param>
	/// <param name="trimTrailingZeros">끝 0 제거</param>
	public static void SetBigNumber(
		this TMP_Text label,
		BigNumber num,
		int decimals = 2,
		string[] suffixes = null,
		bool fallbackScientific = true,
		bool trimTrailingZeros = true)
	{
		if (label == null) return;
		suffixes ??= DefaultSuffix;

		label.text = ToAbbrevString(num, decimals, suffixes, fallbackScientific, trimTrailingZeros);
	}

	/// <summary>
	/// BigNumber → 축약 문자열로 변환.
	/// (UI 없이 문자열만 필요할 때)
	/// </summary>
	public static string ToAbbrevString(
		BigNumber num,
		int decimals = 2,
		string[] suffixes = null,
		bool fallbackScientific = true,
		bool trimTrailingZeros = true)
	{
		suffixes ??= DefaultSuffix;

		// BigNumber는 1000^exponent 기준으로 정규화되어 있다는 가정
		string unit = (num.exponent >= 0 && num.exponent < suffixes.Length)
			? suffixes[num.exponent]
			: (fallbackScientific ? $"e{num.exponent * 3}" : "");

		string fmt = "F" + Mathf.Clamp(decimals, 0, 6).ToString(CultureInfo.InvariantCulture);
		string val = num.value.ToString(fmt, CultureInfo.InvariantCulture);

		if (trimTrailingZeros && val.Contains("."))
		{
			val = val.TrimEnd('0').TrimEnd('.');
		}

		return unit.Length == 0 ? val : $"{val}{unit}";
	}

	/// <summary>
	/// 코루틴으로 부드럽게 숫자를 갱신합니다 (카운트업/다운 애니메이션).
	/// </summary>
	/// <param name="host">StartCoroutine을 호출할 MonoBehaviour</param>
	/// <param name="label">대상 TMP 텍스트</param>
	/// <param name="from">시작 값</param>
	/// <param name="to">도착 값</param>
	/// <param name="duration">애니메이션 시간(초)</param>
	/// <param name="decimals">표시 소수점</param>
	/// <param name="easing">0~1 입력을 0~1로 변환하는 이징 함수(null이면 선형)</param>
	public static Coroutine TweenBigNumber(
		this MonoBehaviour host,
		TMP_Text label,
		BigNumber from,
		BigNumber to,
		float duration = 0.5f,
		int decimals = 2,
		Func<float, float> easing = null)
	{
		if (host == null || label == null)
			return null;

		return host.StartCoroutine(TweenCR(label, from, to, duration, decimals, easing ?? (t => t)));
	}

	/// <summary>
	/// 일정 주기마다 공급자(Func)를 호출해 최신 값을 표시합니다.
	/// (예: DPS, 재화, 프로그레스 등 실시간 값 갱신)
	/// </summary>
	public static Coroutine BindBigNumber(
		this MonoBehaviour host,
		TMP_Text label,
		Func<BigNumber> valueProvider,
		float interval = 0.1f,
		int decimals = 2)
	{
		if (host == null || label == null || valueProvider == null)
			return null;

		return host.StartCoroutine(BindCR(label, valueProvider, interval, decimals));
	}

	// ----------------- 내부 구현 -----------------

	private static IEnumerator TweenCR(TMP_Text label, BigNumber from, BigNumber to, float duration, int decimals, Func<float, float> easing)
	{
		// 지수 차이가 크면 value만 보간하면 부자연스러우니, 지수도 같이 보간
		float t = 0f;
		double startVal = from.value;
		double endVal = to.value;
		int startExp = from.exponent;
		int endExp = to.exponent;

		while (t < duration)
		{
			t += Time.unscaledDeltaTime;
			float u = Mathf.Clamp01(t / Mathf.Max(0.0001f, duration));
			u = Mathf.Clamp01(easing(u));

			// 지수와 값 보간 (선형). 큰 폭 이동 시에도 자연스럽게 변화.
			double v = Lerp(startVal, endVal, u);
			double e = Lerp(startExp, endExp, u);

			var current = new BigNumber(v, Mathf.RoundToInt((float)e));
			label.SetBigNumber(current, decimals);

			yield return null;
		}

		label.SetBigNumber(to, decimals);
	}

	private static IEnumerator BindCR(TMP_Text label, Func<BigNumber> provider, float interval, int decimals)
	{
		var wait = new WaitForSecondsRealtime(Mathf.Max(0.01f, interval));
		while (true)
		{
			var val = provider();
			label.SetBigNumber(val, decimals);
			yield return wait;
		}
	}

	private static double Lerp(double a, double b, float t) => a + (b - a) * t;

	private static string[] BuildDefaultSuffix()
	{
		// "", A..Z, AA..AZ, BA..BZ (필요하면 더 늘리세요)
		var list = new System.Collections.Generic.List<string> { "" };

		// A..Z
		for (char c = 'A'; c <= 'Z'; c++) list.Add(c.ToString());

		// AA..AZ, BA..BZ
		for (char c1 = 'A'; c1 <= 'B'; c1++)
			for (char c2 = 'A'; c2 <= 'Z'; c2++)
				list.Add($"{c1}{c2}");

		return list.ToArray();
	}
}
