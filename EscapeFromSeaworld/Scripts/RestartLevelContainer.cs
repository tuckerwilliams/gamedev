//using UnityEngine;
//using System.Collections;
//using UnityEngine.SceneManagement;
//
//public class RestartLevelContainer : MonoBehaviour {
//
//	private static RestartLevelContainer instance;
//	public static RestartLevelContainer Instance {get {return instance;} }
//	public GameManager GameManager;
//
//	public LevelInformation levelInfo;
//
//	void Awake()
//	{
//		if (instance != null && instance != this) {
//			Destroy (this.gameObject);
//		} else {
//			instance = this;
//		}
//
//		DontDestroyOnLoad (gameObject);
//		print ("Level Restart Container created!");
//	}
//
//	public void LoadRestartedLevel()
//	{
//		int level = levelInfo.level;
//		if (level < 1 || level > 6) {
//			Debug.LogError ("FATAL ERROR. TRYING TO LOAD INACCESIBLE LEVEL.");
//		}
//
//		switch (level) {
//
//		case 1:
//			Instantiate (GameManager);
//			SceneManager.LoadScene ("Scenes/Level1_WaterTest"); //no stats to reset...
//			break;
//
//		case 2: 
//			SceneManager.LoadScene ("Scenes/Level2_Supermarket");
//			GameManager.Instance.SetLevelStatsOnRestart ();
//			break;
//		case 3: 
//			SceneManager.LoadScene ("Scenes/Level3_Park");
//			GameManager.Instance.SetLevelStatsOnRestart ();
//			break;
//		case 4: 
//			SceneManager.LoadScene ("Scenes/Level4_Sewer");
//			GameManager.Instance.SetLevelStatsOnRestart ();
//			break;
//		case 5: 
//			SceneManager.LoadScene ("Scenes/Level5_Mine");
//			GameManager.Instance.SetLevelStatsOnRestart ();
//			break;
//		case 6: 
//			SceneManager.LoadScene ("Scenes/Level6_Beach");
//			GameManager.Instance.SetLevelStatsOnRestart ();
//			break;
//		default:
//			Debug.LogError ("ERROR There were no matching cases in attempting to load restart level. ERROR \n LEVEL GIVEN = " + levelInfo.level);
//			break;
//		}
//
//	}
//
//	public struct LevelInformation {
//		public int level;
//		public float health;
//		public int inks;
//		public int numInksUsed;
//		public float totalTime;
//	}
//


	//For Use in Other classes

//	public void SendLevelStats()
//	{
//		RestartLevelContainer.Instance.levelInfo.level = level;
//		RestartLevelContainer.Instance.levelInfo.inks = numberOfInks;
//		RestartLevelContainer.Instance.levelInfo.health = health.CurrentVal;
//		//		RestartLevelContainer.Instance.levelInfo.numInksUsed = gameStats.numInksUsed;
//		//		RestartLevelContainer.Instance.levelInfo.totalTime = gameStats.totalTime;
//	}
//
//	//This should be more private, i.e., only accessible by RestartLevelContainer class;
//	public void SetLevelStatsOnRestart()
//	{
//		health.CurrentVal = RestartLevelContainer.Instance.levelInfo.health;
//		level = RestartLevelContainer.Instance.levelInfo.level;
//		numberOfInks = RestartLevelContainer.Instance.levelInfo.level;
//		//		gameStats.numInksUsed = RestartLevelContainer.Instance.levelInfo.level;
//		//		gameStats.totalTime = RestartLevelContainer.Instance.levelInfo.level;
//
//	}

//
//}
