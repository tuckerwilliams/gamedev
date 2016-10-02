using UnityEngine;
using System.Collections;

public class WaterDetector : MonoBehaviour {

	public Player player;
	//GameManager gameManager;
	//Controller2D controller;

	// Use this for initialization
	void Start () {
		//gameManager = GetComponent<GameManager> ();
		//player = GetComponent<Player> ();
		//controller = GetComponent<Controller2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		//Debug.Log ("Water script is updating");
		//Physics2D.Over
	}

	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag == "Ground")
			Debug.Log ("Box collider surrounding player has touched ground.");
		Debug.Log ("Box Collider surrounding player has collided with something.");
		if (other.gameObject.tag == "Water") {
			GameManager.Instance.inWater = true;
			Debug.Log ("COLLISION DETECTED WITH WATER");
			//Once we detect water, disable the collider so you fall through.
			//hit.collider.enabled = false; 
			//Enable bool so rest of system knows your in water
		}
	}

	void OnCollisionExit2D(Collision2D other) {
		if (other.gameObject.tag == "Water") {
			GameManager.Instance.inWater = false;
			Debug.Log ("Collision exit: Leaving water");
		}
	}
}
