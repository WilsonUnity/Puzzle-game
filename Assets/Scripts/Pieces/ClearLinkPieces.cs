using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearLinkPieces : ClearPieces
{

	public bool isRow;

	private void Start()
	{
		
	}

	private void Update()
	{
		
	}

	public override void Clear()
	{
		base.Clear();

		if (isRow)
		{
			piece.Grid.ClearRow(piece.Y);
		}
		else
		{
			piece.Grid.ClearCol(piece.X);
		}
	}
}
