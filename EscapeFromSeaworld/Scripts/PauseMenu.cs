using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

	private static PauseMenu instance;
	public static PauseMenu Instance {get {return instance;} }

	public GameObject Canvas;
	public GameObject Camera;
	bool Paused = false;
	bool OutsidePause = false; //for informing update;

	void Awake()
	{

		if (instance != null && instance != this) {
			Destroy (this.gameObject);
		} else {
			instance = this;
		}

		DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		Canvas.gameObject.SetActive (false);
	}

	void Update ()
	{
		if (Input.GetKeyDown ("escape")) {
			if (Paused == true) {
				Time.timeScale = 1.0f;
				Canvas.gameObject.SetActive (false);
				Paused = false;
			} else {
				Time.timeScale = 0.0f;
				Canvas.gameObject.SetActive (true);
				Paused = true;
			}
		}
	}

	public void SetToPaused()
	{
		Paused = true;
	}

	public void Resume ()
	{
		Time.timeScale = 1.0f;
		Canvas.gameObject.SetActive (false);
		Paused = false;
		OutsidePause = false;
	}

	public bool isPaused ()
	{
		return Paused;
	}

}
