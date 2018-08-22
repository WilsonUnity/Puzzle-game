using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearPieces : MonoBehaviour
{

	public AnimationClip clearpieces;

	private bool isBeginAnimation = false;
	
    public bool IsBeginAnimation
	{
		get { return isBeginAnimation; }
	}

	protected GamePiece piece;
	
	private void Awake()
	{
		piece = this.GetComponent<GamePiece>();
	}

	public virtual void Clear()
	{
		isBeginAnimation = true;
		StartCoroutine(ClearCoroutine());
		piece.Grid.level.OnPieceCleared(piece);
	}

	public IEnumerator ClearCoroutine()
	{
		Animator animator = this.GetComponent<Animator>();
		
		if (animator)
		{
			animator.Play(clearpieces.name);
		}

		yield return new WaitForSeconds(clearpieces.length);
		
		Destroy(gameObject);
	}
}
