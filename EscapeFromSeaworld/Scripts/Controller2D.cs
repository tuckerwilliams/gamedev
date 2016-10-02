
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; //For loading of new scenes

[RequireComponent (typeof (BoxCollider2D))]
public class Controller2D : MonoBehaviour {

	Player player;
	BoxCollider2D collider;
	Rigidbody2D rb2d;
	//GameManager gameManager;
	Animator anim;

	RaycastOrigins raycastOrigins;
	public CollisionInfo collisions;
	public GameObject inkPowerUp;

	public LayerMask collisionMask;

	const float skinWidth = .015f;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	[HideInInspector] public bool badCollision; //boolean for things like oil spill, salt, etc.

	//float swimSpeed = 5;
	float maxClimbAngle = 80;
	float maxDescendAngle = 75;

	float startTime, endTime;

	float horizontalRaySpacing;
	float verticalRaySpacing;

	public bool hasKey = false;
	public float boxMoveSpeed;

	void Start() 
	{
		player = GetComponent<Player> (); //so we can add to player health on water collision.
		collider = GetComponent<BoxCollider2D> ();
		anim = GetComponent<Animator> ();

		badCollision = false;

		//Only need to calculate ray spacing once, because our character is of constant, static size.
		CalculateRaySpacing ();
	}

	public void Move(Vector3 velocity) 
	{
		//Update raycast position points as the player moves in the game world.
		UpdateRaycastOrigins ();
		//Reset collisions for a "blank slate." 
		collisions.Reset ();
		collisions.velocityOld = velocity;

		if (velocity.y < 0)
			DescendSlope (ref velocity);

		if (velocity.x != 0) {
			HorizontalCollisions (ref velocity);
		}
		if (velocity.y != 0) {
			VerticalCollisions (ref velocity);
		}

		anim.SetFloat ("Speed", velocity.x); 

		transform.Translate (velocity);
	}

	void HorizontalCollisions(ref Vector3 velocity) 
	{
		float directionX = Mathf.Sign (velocity.x);
		float rayLength = Mathf.Abs (velocity.x) + skinWidth;

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength,Color.red);

			if (hit) {
				CheckRaycastCollisionLayer(hit, velocity); //Probably no horizontal water layers?
				//Get the angle of collisions for colliding with slopes.
				float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (collisions.descendingSlope) {
					collisions.descendingSlope = false;
					velocity = collisions.velocityOld;
				}

				if (i == 0 && slopeAngle < maxClimbAngle) {
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance - skinWidth;
						velocity.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope (ref velocity, slopeAngle);
					velocity.x += distanceToSlopeStart * directionX; //add back the distance subtracted when we climbed slope.
				}

				if (!collisions.climbingSlope || slopeAngle > maxClimbAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						velocity.y = Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x);
					}

					//If we've hit something, and we're going LEFT, collisions.left = true
					collisions.left = directionX == -1;
					//If we've hit something, and we're going RIGHT, collisions.right = true
					collisions.right = directionX == 1;
				}
			}
		}
	}

	void VerticalCollisions(ref Vector3 velocity) 
	{
		float directionY = Mathf.Sign (velocity.y);
		float rayLength = Mathf.Abs (velocity.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) {
			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength,Color.red);

			if (hit) {
				CheckRaycastCollisionLayer(hit, velocity);
				velocity.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				//If colliding from above, we should adjust the velocity in x direction.
				if (collisions.climbingSlope) {
					velocity.x = Mathf.Abs (velocity.y) / Mathf.Tan (collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x); 
				}

				//If we've hit something, from BELOW, collisions.below = true
				collisions.below = directionY == -1;
				//If we've hit something, from ABOVE, collisions.above = true
				collisions.above = directionY == 1;
			}
		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign (velocity.x);
			rayLength = Mathf.Abs (velocity.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up); 

				//Have we collided with a new slope? If true:
				if (slopeAngle != collisions.slopeAngle) {
					velocity.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
				}
			}
		}
	}

	void ClimbSlope (ref Vector3 velocity, float slopeAngle)
	{
		float moveDistance = Mathf.Abs (velocity.x);
		float climbVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		//IF true, assume we are jumping
		if (velocity.y <= climbVelocityY ) { 
			velocity.y = climbVelocityY;
			velocity.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x); //Multiply by Mathf.Sign to maintain direction whether moving left or right
			collisions.below = true; 
			collisions.climbingSlope = true;

			collisions.slopeAngle = slopeAngle;
		}
	}

	void DescendSlope (ref Vector3 velocity)
	{
		float directionX = Mathf.Sign (velocity.x);
		Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
		RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

		if (hit) {
			
			float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
			if (slopeAngle != 0 && slopeAngle <= maxDescendAngle) {
				//moving down slop 
				if (Mathf.Sign (hit.normal.x) == directionX) {
					//Are we close to the slope?
					if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (velocity.x)) {
						float moveDistance = Mathf.Abs (velocity.x);
						float descendVelocityY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
						float descendVelocityX =  Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x); //Multiply by Mathf.Sign to maintain direction whether moving left or right
						velocity.y -= descendVelocityY;

						collisions.slopeAngle = slopeAngle;
						collisions.descendingSlope = true;
						collisions.below = true;
					}
				}
			}
		}
	}

	void UpdateRaycastOrigins() 
	{
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	void CalculateRaySpacing() 
	{
		Bounds bounds = collider.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue);
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	void CheckRaycastCollisionLayer(RaycastHit2D hit, Vector3 velocity)
	{
		//Collide with a key?
		if (hit.collider.gameObject.layer == 12) {
			hasKey = true;
			Destroy (hit.collider.gameObject);
		}

		//Door and key
		if (hit.collider.gameObject.layer == 13 && hasKey == true) {
			hit.collider.enabled = false;
			StartCoroutine (DestroyKeyHoleAfterSeconds ());

			//Door with no key
		} else if (hit.collider.gameObject.layer == 13 && hasKey == false) {
			GameManager.Instance.HelpText.text = "You need a key!";
			EraseHelpTextAfterSeconds (3f);
		}

		//Collided with a box to move.
		if (hit.collider.gameObject.layer == 16) {
			//float boxMoveSpeed;
			Vector3 boxMoveVector = new Vector3 (velocity.x, 0, 0);
			hit.transform.Translate (boxMoveVector * Time.deltaTime * boxMoveSpeed);
		}

		if (hit.collider.gameObject.layer == 17) {
			Vector3 spawnPosition = hit.collider.gameObject.transform.position;
			Destroy (hit.collider.gameObject);
			GameManager.Instance.IncreaseInks ();
			StartCoroutine(WaitForSecondsToSpawnInk(spawnPosition, 5f));

		}
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.gameObject.tag == "DoorToLevel2") {
			Reset ();
			GameManager.Instance.IncreaseLevel ();
			SceneManager.LoadScene("Scenes/Level2_Supermarket");
			GameManager.Instance.NewLevelLoaded ();
		}

		if (other.gameObject.tag == "DoorToLevel3") {
			Reset ();
			GameManager.Instance.IncreaseLevel ();
			SceneManager.LoadScene("Scenes/Level3_Park");
			GameManager.Instance.NewLevelLoaded ();
		}

		if (other.gameObject.tag == "DoorToLevel4") {
			Reset ();
			GameManager.Instance.IncreaseLevel ();
			SceneManager.LoadScene("Scenes/Level4_Sewer");
			GameManager.Instance.NewLevelLoaded ();
		}

		if (other.gameObject.tag == "DoorToLevel5MineShaft") {
			Reset ();
			GameManager.Instance.IncreaseLevel ();
			SceneManager.LoadScene("Scenes/Level5_Mine");
			GameManager.Instance.NewLevelLoaded ();
		}

		if (other.gameObject.tag == "DoorToLevel6") {
			Reset ();
			GameManager.Instance.IncreaseLevel ();
			SceneManager.LoadScene("Scenes/Level6_Beach");
			GameManager.Instance.NewLevelLoaded ();
		}

		if (other.gameObject.tag == "Ocean") {
			GameManager.Instance.GameOver ();
			SceneManager.LoadScene("Scenes/YouWin");
			if (Input.GetKey ("q"))
				Application.Quit ();
		}

		if (other.gameObject.tag == "HelpTextHint") {
			int level = GameManager.Instance.GetLevelNumber ();
			bool stringCancel = false;
			if ((level == 1 && hasKey) || (level == 2 && GameManager.Instance.currentlyInking) || (level == 3 && !badCollision))
				stringCancel = true;
			if (level == 3) {
				GameManager.Instance.HelpText.color = new Color (255f, 255f, 255f);
			}
			
			GameManager.Instance.HelpText.text = HelpTextOutput(level, stringCancel);

			Destroy (other.gameObject);
			StartCoroutine (EraseHelpTextAfterSeconds (4f));
			GameManager.Instance.HelpText.color = new Color (0f, 0f, 0f);
		}

		if (other.gameObject.tag == "DoorToOtherSideOfColumn") {
			print ("You're colliding with the door. You should be other the other side. But you should have a key!");
			player.transform.position.Set (24.5f, player.transform.position.y, player.transform.position.z);
		}

		if (other.gameObject.tag == "Water") {
			GameManager.Instance.inWater = true;
		}
			
		if (other.gameObject.tag == "Oil") {
			badCollision = true;
			StartCoroutine(CollisionWithOil(1, .4f, other.gameObject.tag) );
		}

		if (other.gameObject.tag == "FellToDeath") {
			GameManager.Instance.GameOver ();
			SceneManager.LoadScene ("Scenes/Menus/FalltoDeath");
		}

	}
		
	void OnTriggerExit2D(Collider2D other) 
	{
		if (other.gameObject.tag == "Water") {
			GameManager.Instance.inWater = false;
		}

		if (other.gameObject.tag == "Oil") {
			badCollision = false;
		}
	}

	IEnumerator CollisionWithOil(int healthFactor, float speedFactor, string collisionTag)
	{
		float oldMoveSpeed = player.moveSpeed;

		//This is made outside the while loop so they aren't incrementally updated. If it were the case, both would quickly reach the limit 0.
		player.moveSpeed *= speedFactor;

		while (badCollision) {
			GameManager.Instance.DecreaseHealthFromCollision (healthFactor);
			yield return new WaitForSecondsRealtime (1f); //remove 1 health every 2 seconds while true.
		}
		player.moveSpeed = oldMoveSpeed;
	}
		
	void LoadNewScene(int sceneNum)
	{
		SceneManager.LoadScene (sceneNum);
		GameManager.Instance.IncreaseLevel ();
	}

	IEnumerator EraseHelpTextAfterSeconds(float seconds)
	{
		yield return new WaitForSecondsRealtime (seconds);
		GameManager.Instance.HelpText.text = "";
	}
		

	//enumerator method because didn't work otherwise.
	IEnumerator DestroyKeyHoleAfterSeconds()
	{
		yield return new WaitForSecondsRealtime (2f);
		hasKey = false;
	}

	IEnumerator EnableWaterAfterSeconds()
	{
		yield return new WaitForSecondsRealtime (.2f);
		GameManager.Instance.inWater = true;
	}

	string HelpTextOutput(int level, bool stringCancel) 
	{

		string textToDisplay = "";

		switch (level) {
		case 1:
			textToDisplay = "Hint: Try finding a key.";
			break;
		case 2:
			textToDisplay = "Hint: Press 'Enter'! for an Ink Blast";
			break;
		case 3:
			textToDisplay = "Oil slows you down. Use Ink to jump over it!";
			break;
		default:
			textToDisplay = "";
			break;
		}
		//This condition says: If something happens (i.e., they already have the key), don't display the key message.
		if (stringCancel)
			return "";
		else
			return textToDisplay;
	}

	IEnumerator WaitForSecondsToSpawnInk(Vector3 spawnPosition, float interval)
	{
		yield return new WaitForSecondsRealtime (interval);
		Instantiate (inkPowerUp, spawnPosition, Quaternion.identity);
	}

	void Reset() 
	{
		GameManager.Instance.HelpText.text = "";
		GameManager.Instance.inWater = false;
		GameManager.Instance.currentlyInking = false;
	}
		
	struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}


	public struct CollisionInfo {
		public bool above, below;
		public bool left, right; 

		public bool climbingSlope, descendingSlope;
		public float slopeAngle, slopeAngleOld; //where slopeAngleOld is slopeAngle from previous frame

		public Vector3 velocityOld;

		public void Reset() {
			above = below = false;
			left = right = false; 
			climbingSlope = descendingSlope = false;
			 
			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}
	}
}
