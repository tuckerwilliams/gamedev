using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStats : MonoBehaviour {

	private static GameStats instance;
	public static GameStats Instance {get {return instance;} }

	public Text gameStatsText;

	void Awake()
	{
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
		} else {
			instance = this;
		}

		DontDestroyOnLoad (gameObject);
		print ("GameStats object created!");

	}

	// Update is called once per frame
	void Update () 
	{
//		if ( SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Scenes/Menus/OutOfTime") || (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Scenes/Menus/OutOfOxygen")) )
//			DisplayStats();
	}
//
//	void DisplayStats()
//	{
//		gameStatsText.text = "Game Stats: \n \t \t Time Elapsed: " + (int)GameManager.Instance.gameStats.totalTime + " seconds";
//		gameStatsText.text += "\n \t \t Ink Blasts Used: " + GameManager.Instance.gameStats.numInksUsed;
//	}
}
