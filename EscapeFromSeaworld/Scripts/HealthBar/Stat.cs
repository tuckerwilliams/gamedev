using UnityEngine;
using System.Collections;
using System;

[Serializable]
public class Stat 
{
	[SerializeField]
	private HealthBarScript bar;

	[SerializeField]
	public float maxVal;

	[SerializeField]
	public float currentVal;

	public float CurrentVal 
	{
		get
		{
			return currentVal;
		}

		set
		{
			//makes sure no illegal health values. Exp: Negative Health, More than maxVal
			this.currentVal = Mathf.Clamp(value, 0, MaxVal); 
			bar.Value = currentVal;
		}
	}

	public float MaxVal
	{
		get
		{
			return maxVal;
		}

		set
		{
			this.maxVal = value;
			//UI.Instance.MaxValue = maxVal;
			bar.MaxValue = maxVal;
		}
	}

	public void Initialize()
	{
		this.MaxVal = maxVal;
		this.CurrentVal = currentVal;
	}
}
