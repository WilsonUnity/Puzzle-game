using UnityEngine;
using System.Collections;

public class Level : MonoBehaviour {

	public enum LevelType
	{
		TIMER,
		OBSTACLE,
		MOVES,
	};

	public Grid grid;
	public HUD hud;

	public int score1Star;
	public int score2Star;
	public int score3Star;

	private void Start()
	{
		hud.SetScore (currentScore);
	}

	protected LevelType type;

	public LevelType Type {
		get { return type; }
	}

	protected int currentScore;
 
	public virtual void GameWin()
	{
		grid.GameOver();
		hud.OnGameWin (currentScore);
	}

	public virtual void GameLose()
	{
		grid.GameOver();
		hud.OnGameLose ();
	}

	public virtual void OnMove()
	{
         Debug.Log("you move");
	}

	public virtual void OnPieceCleared(GamePiece piece)
	{
		currentScore += piece.score;
		hud.SetScore (currentScore);
	}
}
