using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {

	private static AudioManager instance;
	public static AudioManager Instance {get {return instance;} }



	public AudioSource openingCreditsTheme;
	public AudioSource daylightTheme;
	public AudioSource dungeonTheme;
	public AudioSource finalTheme;

	void Awake() 
	{
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
		} else {
			instance = this;
		}

		openingCreditsTheme.Play ();

		DontDestroyOnLoad (gameObject);
	}

	public void StopSong(int index)
	{

		switch (index) {

		case 1:
			openingCreditsTheme.Stop ();
			break;
		case 2:
			daylightTheme.Stop ();
			break;
		case 3:
			dungeonTheme.Stop ();
			break;
		case 4:
			break;

		}

	}

	public void StartSong(int index) 
	{
		switch (index) {

		case 2:
			daylightTheme.Play ();
			break;
		case 3:
			dungeonTheme.Play ();
			break;
		}
	}
}