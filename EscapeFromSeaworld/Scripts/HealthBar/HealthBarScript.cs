using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarScript : MonoBehaviour {

	private float fillAmount;

	[SerializeField]
	private float lerpSpeed;

	[SerializeField]
	private Image content;

	[SerializeField]
	private Text valueText;

	public float MaxValue { get; set; }

	public float Value {
		set {

			valueText.text = value.ToString (); //value to display on health bar
			fillAmount = AdjustHealthVal (value, MaxValue, 1); //converts
		}
	}
		
	
	// Update is called once per frame
	void Update () {
		handleBar ();
	}

	private void handleBar()
	{
		if (fillAmount != content.fillAmount) {
			//Lerp is used to make a smoother transitions of health bar transitions
			content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime *lerpSpeed);
		}
	}

	private float AdjustHealthVal(float value, float inMax, float outMax)
	{
		return value * outMax / inMax;
		//Converts the current health to a decimal (percentage) to be used in displaying health bar
		// 25* 1 / 100
		// 80 * 1 / 100 = 0.8

	}
}
