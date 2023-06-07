using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.
using UnityEngine.SceneManagement;

	//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
	public class Player : MovingObject
	{
		public float restartLevelDelay = 1f;		//Delay time in seconds to restart level.
		public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
		public int enemyDamage = 50;
		private Animator animator;					//Used to store a reference to the Player's animator component.
		private int life;                           //Used to store player food points total during level.
		public Text LifePointText;

		
		
		//Start overrides the Start function of MovingObject
		protected override void Start ()
		{
			//Get a component reference to the Player's animator component
			animator = GetComponent<Animator>();
			
			//Get the current food point total stored in GameManager.instance between levels.
			life = GameManager.instance.playerLifePoints;
			
		 
			LifePointText.text = "LifePoint = " + life;

			//Call the Start function of the MovingObject base class.
			base.Start ();
		}

		public int GetEnnemyDamage(){
			return enemyDamage;
		}
		
		
		//This function is called when the behaviour becomes disabled or inactive.
		private void OnDisable ()
		{
			//When Player object is disabled, store the current local food total in the GameManager so it can be re-loaded in next level.
			GameManager.instance.playerLifePoints = life;
		}

	    
		
		private void Update ()
		{
		if (!GameManager.instance.playersTurn) return;

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int) Input.GetAxisRaw ("Horizontal");
		vertical = (int) Input.GetAxisRaw ("Vertical");

		if (horizontal != 0)
			vertical = 0;
		if (horizontal != 0 || vertical != 0)
			AttemptMove<Wall> (horizontal, vertical);
		}

	//AttemptMove overrides the AttemptMove function in the base class MovingObject
	//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
		protected override void AttemptMove <T> (int xDir, int yDir)
		{
		
		//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
		base.AttemptMove <T> (xDir, yDir);

		//Hit allows us to reference the result of the Linecast done in Move.
		RaycastHit2D hit;

		//Since the player has moved and lost food points, check if the game has ended.
		CheckIfGameOver ();

		//Set the playersTurn boolean of GameManager to false now that players turn is over.
		GameManager.instance.playersTurn = false;
		}

		//CheckIfGameOver checks if the player is out of food points and if so, ends the game.
		private void CheckIfGameOver ()
		{
			//Check if food point total is less than or equal to zero.
		if (life <= 0) {
			//Call the GameOver function of GameManager.
			GameManager.instance.GameOver ();
		}
		}

		
	private void OnTriggerEnter2D(Collider2D other){
		if (other.tag == "Exit") {
			Invoke ("Restart", restartLevelDelay);
			enabled = false;
		}
	}

		
		//OnCantMove overrides the abstract function OnCantMove in MovingObject.
		//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
		protected override void OnCantMove <T> (T component)
		{
		
		Wall hitWall = component as Wall;
		if (hitWall != null)
		{
			// Appeler la fonction DamageWall du mur
			hitWall.DamageWall(wallDamage);
			// Déclencher l'animation d'attaque du joueur
			animator.SetTrigger("playerChop");
			return;
		}

		// Vérifier si le composant est un ennemi
		Enemy hitEnemy = component as Enemy;
		if (hitEnemy != null)
		{
			// Appeler la fonction DamageWall du mur
			hitEnemy.DamageEnemy(enemyDamage);
			// Déclencher l'animation d'attaque du joueur
			animator.SetTrigger("playerChop");
			return;
		}	
	
		}


	//Restart reloads the scene when called.
		private void Restart ()
		{
		SceneManager.LoadScene(Application.loadedLevel);
		}

		//LoseFood is called when an enemy attacks the player.
		//It takes a parameter loss which specifies how many points to lose.
		public void LoseLife (int loss)
		{
		//Set the trigger for the player animator to transition to the playerHit animation.
		animator.SetTrigger ("playerHit");

		//Subtract lost food points from the players total.
		life -= loss;

		LifePointText.text = "LifePoint = " + life;

		//Check to see if game has ended.
		CheckIfGameOver ();
		}

	}

