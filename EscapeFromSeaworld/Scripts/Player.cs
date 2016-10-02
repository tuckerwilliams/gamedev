using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Controller2D))]
public class Player : MonoBehaviour {

	private static Player instance;
	public static Player Instance {get {return instance;} }

	Controller2D controller;
	private ProduceInk ink;
	SpriteRenderer spriteRenderer;

	float gravity;
	[HideInInspector] public float jumpVelocity;

	//For smoothing x velocity. Time to smooth.
	//float accelerationTimeAirborne = .2f;
	float accelerationTimeGrounded = .1f;

	//float swimSpeed = 5f;
	public float moveSpeed = 6;
	public float jumpHeight = 4;
	public float timeToJumpApex = .4f;

	//For smoothing velocity in the x direction so it is smoother: entirely handled in Mathf.SmoothDamp; need it for a reference.
	float velocityXSmoothing; 
	Vector3 velocity;

	void Start () 
	{
		if (instance != null && instance != this) {
			Destroy (this.gameObject);
		} else {
			instance = this;
		}
			
		spriteRenderer = GetComponent<SpriteRenderer> ();
		controller = GetComponent<Controller2D> ();
		ink = GetComponent<ProduceInk> ();
	
		//Gravity (negative) computed from kinematic equations:  deltaDistance = initialVelocity * time + (acceleration * time^2)/2
		gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex,2); 
		jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
	}


	// Update is called once per frame
	void Update () 
	{
		// If we're grounded, reset velocity in y direction so gravity doesn't accumulate.
		// If you turn this code block off, the character falls off the screen extremely fast because he "builds up" gravity velocity.
		if (controller.collisions.above || controller.collisions.below) {
			velocity.y = 0;
		}

		Vector2 input = new Vector2 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"));

		//If the player is colliding from below (i.e., standing on something) and presses space, then jump.
		if (Input.GetKeyDown (KeyCode.Space) && controller.collisions.below) {
			velocity.y = jumpVelocity; 
		}

		//If the player is on the ground, has an ink powerup and presses control his jumpVeloctiy and moveSpeed increase for 2 seconds. 
		if (Input.GetKeyDown (KeyCode.Return) && controller.collisions.below) {
			ink.inkPowerUp ();
		}

		if (velocity.x < 0 && this.spriteRenderer.flipX == false)
			this.spriteRenderer.flipX = true;
		else if (velocity.x > 0 && this.spriteRenderer.flipX == true)
			this.spriteRenderer.flipX = false;

		float targetVelocityX = input.x * moveSpeed;
		velocity.x = Mathf.SmoothDamp (velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeGrounded );
		velocity.y += gravity * Time.deltaTime;
		controller.Move (velocity * Time.deltaTime);
	}
		
	public float getJumpVelocity () {
		return jumpVelocity;
	}

	public void multiplyJumpVelocity (float mult) {
		jumpVelocity = jumpVelocity * mult;
	}

	public float getMoveSpeed () {
		return moveSpeed;
	}

	public void multiplyMoveSpeed (float mult) {
		moveSpeed = moveSpeed * mult;
	}

	public void DecreaseSpeedFromCollision(int factor) {
		moveSpeed *= factor;
	}

	public void DecreaseJumpVelocityFromCollision(int factor) {
		jumpVelocity *= factor;
	}
}