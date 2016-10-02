using UnityEngine;
using System.Collections;
using UnityEngine.UI; //Allows us to use UI.
using UnityEngine.SceneManagement;

//Credit for "Singleton" Awake set-up: Unity intermediate guide, "2D Roguelike" tutorial.

/* 1. Why say DontDestroyOnLoad(gameObject), rather than DontDestroyOnLoad(this)?
 * 2. Maybe cool idea: Track obstacles, or distance jumped, or whatever, and tell the player when they die (or win) how they did!
 * 
 * 
 * */

public class GameManager : MonoBehaviour {

	private static GameManager instance;
	public static GameManager Instance {get {return instance;} }

	[SerializeField] private Stat health;

	bool restart;

	public Text levelText;
	public Text inksLeft; 
	public Text timeText;
	public Text HelpText;

	public int time = 0;

	[HideInInspector] public bool inkPresent;
	[HideInInspector] public bool inWater;
	[HideInInspector] public bool currentlyInking;
	[HideInInspector] public int numberOfInks;

//	public GameStats gameStats;

	float timerCountDown = 0f;

	private bool newLevelLoaded = false;
	private int level = 1;
	private int maxNumInks = 6;

	void Awake() 
	{
		health.Initialize ();
		//InitGStatsStruct ();

		if (instance != null && instance != this) {
			Destroy (this.gameObject);
		} else {
			instance = this;
		}

		DontDestroyOnLoad(gameObject);
	}
		
	// Use this for initialization
	void Start () 
	{
		inkPresent = false;
		inWater = false;
		currentlyInking = false;
		numberOfInks = 5;

		time = MaxLevelTimer (level);

		inksLeft.text = "Inks Boosts: " + GameManager.Instance.GetNumberOfInks(); //value to display on health bar
		levelText.text = "Level: " + GameManager.Instance.GetLevelNumber();
		timeText.text = "Time: " + time;

		StartCoroutine (DecreaseHealth ());
	}
	
	// Update is called once per frame
	void Update () 
	{
		
		//If a new level is loaded, get the timer for that level, then set newLevelLoaded to false.
		if (newLevelLoaded) {
			time = MaxLevelTimer (level);

			timerCountDown = 0f;
			newLevelLoaded = false;
		}

		//Need to increment in order to subtract for time text.
		timerCountDown += Time.deltaTime;
		//gameStats.totalTime += timerCountDown;
		CheckIfTimedOut (time - (int)timerCountDown);

		inksLeft.text = "Inks Boosts: " + GameManager.Instance.GetNumberOfInks(); //value to display on health bar
		levelText.text = "Level: " + GameManager.Instance.GetLevelNumber();
		timeText.text = "Time: " + (time - (int)timerCountDown);
	}
		

	public IEnumerator DecreaseHealth()
	{
		while (health.CurrentVal >= 0) {
			CheckIfNoOxygen ();

			// if game is paused, do not decrease health
			while (PauseMenu.Instance.isPaused () == true) {
				yield return null;
			}
				
			if (inWater) {
				health.CurrentVal += 5;
				yield return new WaitForSecondsRealtime (3.5f);
			}
			else {
				health.CurrentVal--;
				yield return new WaitForSecondsRealtime (1f);
			}
		}
	}
		
	//Increase 1 if at 4; otherwise, increase at most by 2.
	public void IncreaseInks() 
	{
		if (numberOfInks == maxNumInks) {
			return;
		}
		for (int i = 0; i <= 1; i++) {
			if (numberOfInks + 1 < maxNumInks)
				numberOfInks += 1;
		}
	}
				
	void CheckIfTimedOut(int timeDifference) 
	{
		if (timeDifference <= 0) {
			StopCoroutine (DecreaseHealth ());
			GameOver ();
			SceneManager.LoadScene ("Scenes/Menus/OutOfTime");
		}
	}

	// Checks if there is no oxygen and if so 
	void CheckIfNoOxygen() 
	{
		if (health.CurrentVal <= 0) {
			StopCoroutine (DecreaseHealth ());
			GameOver ();
			SceneManager.LoadScene ("Scenes/Menus/OutOfTime");
		}
	}

	//Switch statement function to determine time alloted on each level.
	public int MaxLevelTimer(int level)
	{

		int timer;

		switch (level) {

		case 1:
			timer = 40;;
			break;
		case 2:
			timer = 40;
			break;
		case 3:
			timer = 40;
			break;
		case 4:
			timer = 75;
			break;
		case 5:
			timer = 65;
			break;
		case 6:
			timer = 30;
			break;
		default:
			timer = 60;
			break;
		}
		return timer;
	}

	//Function to decrease health when colliding with oil
	public void DecreaseHealthFromCollision(int factor) {
		health.currentVal -= factor;
	}
		
	//GameOver is called when the player reaches 0 health / oxygen
	public void GameOver()
	{
		Destroy(gameObject);
	}

	public void IncreaseLevel() {
		level++;
	}

	public bool NewLevelLoaded()
	{
		newLevelLoaded = true;
		return newLevelLoaded;
	}

	public float GetHealth()
	{
		return health.CurrentVal;
	}

	public int GetNumberOfInks() {
		return numberOfInks;
	}

	public int GetLevelNumber() {
		return level;
	}
}
