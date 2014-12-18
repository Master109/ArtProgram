using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour
{
	public float maxSpeed = 10f;
	public bool facingRight = true;
	float move = 0f;
	bool grounded = false;
	float groundCheckRadius;
	public LayerMask whatIsGround;
	public int jumpForce = 500;
	//public Transform wallCheck1;
	//public Transform wallCheck2;
	float wallCheckRadius = 0.375f;
	public float groundHoverAmount = .0001f;
	public GUISkin guiSkin1;
	public int pushOfWallForce = 500;
	public float extraXVel = 0;
	bool onWall = false;
	public bool paused;
	public Vector2 spawnLoc;

	void Awake ()
	{

	}

	// Use this for initialization
	void Start ()
	{
		//jumpForce *= transform.lossyScale.x;
		//maxSpeed *= transform.lossyScale.x;
		spawnLoc = transform.position;
		Input.ResetInputAxes();
		rigidbody.velocity = Vector2.zero;
		groundCheckRadius = transform.lossyScale.x * .0375f;
	}

	public void Reset ()
	{
		transform.position = spawnLoc;
		Input.ResetInputAxes();
		rigidbody.velocity = Vector2.zero;
	}

	// Update is called once per frame
	void FixedUpdate ()
	{
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("GroundCheck"))
			if (go.transform.IsChildOf(transform))
			{
				grounded = Physics.CheckSphere(go.transform.position, groundCheckRadius, whatIsGround);
				if (grounded)
				{
					transform.position = new Vector2(transform.position.x, transform.position.y + groundHoverAmount);
					break;
				}
			}
		move = Input.GetAxis("Horizontal") * maxSpeed;
		if (!onWall && ((move > 0 && !facingRight) || (move < 0 && facingRight)))
			Flip ();
		/*
		if (move != 0 && Physics2D.OverlapCircle(wallCheck1.position, wallCheckRadius, whatIsGround) && !Physics2D.OverlapCircle(wallCheck2.position, wallCheckRadius, whatIsGround))
			transform.position = new Vector2(transform.position.x, transform.position.y + Vector2.Distance(wallCheck1.position, wallCheck2.position) + 0.1f);
			*/
		///*
		ArrayList array = new ArrayList ();
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("WallCheck"))
		{
			if (go.transform.IsChildOf(transform) && Physics.CheckSphere(go.transform.position, wallCheckRadius, whatIsGround))
			{
				array.Clear ();
				array.AddRange(Physics.OverlapSphere(go.transform.position, wallCheckRadius, whatIsGround));
				bool b = true;
				foreach (Collider c in array)
				{
					if (c.name.Contains("(Wall Jump)"))
						b = false;
				}
				if (b)
				{
					extraXVel = 0;
					move = 0;
					break;
				}
			}
		}
		onWall = false;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("WallCheck"))
		{
			if (go.transform.IsChildOf(transform) && Physics.CheckSphere(go.transform.position, wallCheckRadius, whatIsGround))
			{
				array.Clear ();
				array.AddRange(Physics.OverlapSphere(go.transform.position, wallCheckRadius, whatIsGround));
				bool b = true;
				foreach (Collider c in array)
				{
					if (!c.name.Contains("(Wall Jump)"))
						b = false;
				}
				if (b)
				{
					onWall = true;
					extraXVel = 0;
					int x = 0;
					if (transform.position.x > go.transform.position.x)
						x = 1;
					else
						x = -1;
					if ((move > 0 && x < 0) || (move < 0 && x > 0))
						move = 0;
					if (Input.GetAxisRaw("Horizontal") == x && Input.GetAxisRaw("Jump") == 1)
					{
						rigidbody.velocity = Vector2.zero;
						rigidbody.AddForce(Vector2.up * jumpForce);
						extraXVel = x * pushOfWallForce;
						grounded = false;
						onWall = false;
						break;
					}
				}
			}
		}
		extraXVel *= rigidbody.drag;
		//*/
		if (move != 0)
			rigidbody.velocity = new Vector2(move + extraXVel, rigidbody.velocity.y);
	}
	
	void Update ()
	{
		if (grounded && Input.GetKeyDown(KeyCode.Space))
		{
			rigidbody.AddForce(Vector2.up * jumpForce);
			grounded = false;
		}
	}

	void Flip () 
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	void OnGUI ()
	{
		/*
		GUI.skin = guiSkin2;
		if (paused)
		{
			time = Time.fixedTime - time;
			if (GUI.Button(new Rect(0, 0, 100, 25), "Resume"))
			{
				Time.timeScale = 1;
				paused = false;
			}
			else if (!showTimer && GUI.Button(new Rect(0, 75, 150, 25), "Show Speedrun Timer"))
			{
				showTimer = true;
			}
			else if (showTimer && GUI.Button(new Rect(0, 75, 150, 25), "Hide Speedrun Timer"))
			{
				showTimer = false;
			}
		}
		else
		{
			Time.timeScale = 1;
			GUI.skin = guiSkin1;
			if (showTimer)
				GUI.Label(new Rect(0, 25, Screen.width, 50), "" + Mathf.Round(displayTime / numOfDecimalPlaces) * numOfDecimalPlaces);
			if (GUI.Button(new Rect(0, 0, 100, 25), "Menu"))
			{
				paused = true;
				Time.timeScale = 0;
			}
		}
		*/
	}
}