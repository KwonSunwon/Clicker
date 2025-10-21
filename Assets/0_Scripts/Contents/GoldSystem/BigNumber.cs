using System;
using UnityEngine;

[Serializable]
public struct BigNumber : IComparable<BigNumber>
{
	public double value;   // 실제 값 (10^100 같은건 표현 불가하니 지수 사용)
	public int exponent;   // 지수 (10 단위로 몇 번 곱해졌는지)

	private const int BaseNum = 1000; // 1000 넘어가면 지수 올림

	public BigNumber(double val, int exp = 0)
	{
		value = val;
		exponent = exp;
		Normalize();
	}

	public BigNumber(string str)
	{
		value = 0;
		exponent = 0;

		if (string.IsNullOrWhiteSpace(str))
		{
			value = 0;
			exponent = 0;
			return;
		}

		str = str.Trim();

		// e표기법 (예: "1.23e6")
		if (str.Contains("e", StringComparison.OrdinalIgnoreCase))
		{
			if (double.TryParse(str, System.Globalization.NumberStyles.Float, null, out double parsed))
			{
				// 1e6 -> value 1, exponent = 6 * (log10(1000)) 변환
				double exp10 = Math.Log10(parsed);
				exponent = (int)Math.Floor(exp10 / 3);
				value = parsed / Math.Pow(BaseNum, exponent);
			}
			else
			{
				value = 0;
				exponent = 0;
			}
			return;
		}

		// 단위표기 (A,B,C,...)
		string[] suffix = { "", "A", "B", "C", "D", "E", "F", "G",
							"H","I","J","K","L","M","N","O","P","Q",
							"R","S","T","U","V","W","X","Y","Z",
							"AA","AB","AC","AD","AE","AF" };

		for (int i = suffix.Length - 1; i >= 0; i--)
		{
			if (suffix[i].Length > 0 && str.EndsWith(suffix[i], StringComparison.OrdinalIgnoreCase))
			{
				exponent = i;
				str = str.Substring(0, str.Length - suffix[i].Length);
				break;
			}
		}

		if (!double.TryParse(str, out value))
			value = 0;

		Normalize();
	}

	/// <summary> 값 정규화 (1000 이상이면 exponent 증가) </summary>
	private void Normalize()
	{
		while (value >= BaseNum)
		{
			value /= BaseNum;
			exponent++;
		}
		while (value > 0 && value < 1)
		{
			value *= BaseNum;
			exponent--;
		}
	}

	// 덧셈
	public static BigNumber operator +(BigNumber a, BigNumber b)
	{
		if (a.exponent == b.exponent)
			return new BigNumber(a.value + b.value, a.exponent);

		// 큰 지수에 맞춰서 작은 쪽을 변환
		if (a.exponent > b.exponent)
		{
			double diff = a.exponent - b.exponent;
			return new BigNumber(a.value + b.value / Math.Pow(BaseNum, diff), a.exponent);
		}
		else
		{
			double diff = b.exponent - a.exponent;
			return new BigNumber(b.value + a.value / Math.Pow(BaseNum, diff), b.exponent);
		}
	}

	// 뺄셈
	public static BigNumber operator -(BigNumber a, BigNumber b)
	{
		if (a.exponent == b.exponent)
			return new BigNumber(a.value - b.value, a.exponent);

		if (a.exponent > b.exponent)
		{
			double diff = a.exponent - b.exponent;
			return new BigNumber(a.value - b.value / Math.Pow(BaseNum, diff), a.exponent);
		}
		else
		{
			double diff = b.exponent - a.exponent;
			return new BigNumber(a.value / Math.Pow(BaseNum, diff) - b.value, b.exponent);
		}
	}
	// 곱셈
	public static BigNumber operator *(BigNumber a, BigNumber b)
	{
		// (a.value * b.value) × 1000^(a.exp + b.exp)
		return new BigNumber(a.value * b.value, a.exponent + b.exponent);
	}
	public static BigNumber operator *(BigNumber a, double k) => new BigNumber(a.value * k, a.exponent);
	public static BigNumber operator *(double k, BigNumber a) => new BigNumber(a.value * k, a.exponent);

	public static BigNumber operator /(BigNumber a, BigNumber b)
	{
		if (b.value == 0) throw new DivideByZeroException();
		// (a.value / b.value) × 1000^(a.exp - b.exp)
		return new BigNumber(a.value / b.value, a.exponent - b.exponent);
	}
	public static BigNumber operator /(BigNumber a, double k)
	{
		if (k == 0) throw new DivideByZeroException();
		return new BigNumber(a.value / k, a.exponent);
	}



	// 비교
	public int CompareTo(BigNumber other)
	{
		if (this.exponent != other.exponent)
			return this.exponent.CompareTo(other.exponent);
		return this.value.CompareTo(other.value);
	}

	// 문자열 변환 (단위 붙여서 표시)
	public override string ToString()
	{
		string[] suffix = { "", "A", "B", "C", "D", "E", "F", "G",
							"H","I","J","K","L","M","N","O","P","Q",
							"R","S","T","U","V","W","X","Y","Z",
							"AA","AB","AC","AD","AE","AF" };

		string unit = exponent < suffix.Length ? suffix[exponent] : $"e{exponent * 3}";
		return $"{value:F2}{unit}";
	}
}