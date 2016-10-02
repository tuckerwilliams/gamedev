using UnityEngine;
using System.Collections;
using UnityEngine.UI; //Allows us to use UI.
using UnityEngine.SceneManagement;

public class StoryToMenu : MonoBehaviour {

	public Text storyText;

	// Use this for initialization
	void Start () {
		StartCoroutine (DisplayTextGradually ());

		//If button to skip story is pressed,
		// StopCoroutine(DisplayTextGradually() );
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator DisplayTextGradually()
	{
		int i = 0;
		while (i <= 6) {
			storyText.text += textSwitchStatement (i) + "\n";
			i++;
			if (i == 2)
				yield return new WaitForSecondsRealtime (4f);
			else
				yield return new WaitForSecondsRealtime (2.9f);

		}

		yield return new WaitForSecondsRealtime (1.5f);
		SceneManager.LoadScene ("Scenes/Menus/HomeMenu");
	}

	string textSwitchStatement(int counter) 
	{
		string textToReturn = "";
		switch (counter) {
		case 0:
			textToReturn = "You are Ceph. \n";
			break;
		case 1:
			textToReturn = "You're escaping the aquarium to return to your family. \n";
			break;
		case 2:
			textToReturn = "But time is running out.\n";
			break;
		case 3: 
			textToReturn = "You need to watch both your oxygen and the time. \n";
			break;
		case 4: 
			textToReturn = "The aquarium staff is close behind.\n";
			break;
		case 5:
			textToReturn = "You need to make it to the ocean before you get caught, or worse...\n";
			break;
		case 6:
			textToReturn = "Good luck.";
			break;
		}
		return textToReturn;
	}
}
