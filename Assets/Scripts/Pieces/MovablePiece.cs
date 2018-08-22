using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class MovablePiece : MonoBehaviour
{


	private GamePiece piece;
	private IEnumerator movecoroutine;

	private void Awake()
	{
		piece = GetComponent<GamePiece>();
	}

	public void Move(int x,int y,float time)
	{
        movecoroutine = MoveCoroutine(x, y, time);
		StartCoroutine(movecoroutine);
	}

	public IEnumerator MoveCoroutine(int x,int y,float time)
	{
		piece.X = x;
		piece.Y = y;
		
		Vector2 StartPos = transform.position;
		Vector2 endPos = piece.Grid.GetWorldPosition(x, y);

		for (float t = 0; t < time; t += Time.deltaTime)
		{
			transform.position = Vector3.Lerp(StartPos, endPos, t / time);
			yield return 0;
		}
	}
}
