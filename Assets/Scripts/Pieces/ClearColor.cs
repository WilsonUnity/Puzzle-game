using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearColor : ClearPieces
{

	private ColorPieces.ColorType color;

	public ColorPieces.ColorType Color
	{
		get { return color; }
		set { color = value; }
	}

	public override void Clear()
	{
		base.Clear();
		piece.Grid.ClearColor(color);
	}
}
