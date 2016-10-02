using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProduceInk : MonoBehaviour {

	Player player;

	void Start () 
	{
		player = GetComponent<Player> ();
	}

	public void inkPowerUp () 
	{
		if (!GameManager.Instance.currentlyInking && GameManager.Instance.numberOfInks != 0) {
			GetComponent<SpriteRenderer>().color = Color.black;
			//makes sure you can not call multiple ink powerups at once
			GameManager.Instance.currentlyInking = true;

			//increases values
			player.multiplyJumpVelocity (1.3f);
			player.multiplyMoveSpeed (1.7f);

			GameManager.Instance.numberOfInks--;
			//GameManager.Instance.gameStats.numInksUsed++;

			Invoke ("DeactiveInkPowerup", 4);
		}

	}
	public void DeactiveInkPowerup() 
	{

		//return values back to normal
		player.multiplyJumpVelocity ((1/1.3f)); 
		player.multiplyMoveSpeed ((1/1.7f));

		GetComponent<SpriteRenderer>().color = Color.white;

		//can use another powerup
		GameManager.Instance.currentlyInking = false;
	}
		
}
