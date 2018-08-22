using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour
{

	private int x;
	private int y;
	public int score;

	private SpriteRenderer copySelfSprite;

	public int X
	{
		get { return x; }
		set
		{
			if (IsMovePiece())
			{
				x = value;
			}
		}
	} 

	public int Y
	{
		get { return y; }
		set
		{
			if (IsMovePiece())
			{
				y = value;
			}
		}
	}

	private Grid.PieceType pieceType;
    public Grid.PieceType GridPieceType
	{
		get { return pieceType; }
	}

	private Grid grid;
	
    //1.获取网格信息;2.防止其他类修改数据
	public Grid Grid 
	{
		get { return grid; }
	}

	private MovablePiece movablePiece;
	public MovablePiece Movablepiece
	{
		get { return movablePiece; }
	}

	private ColorPieces colorpieces;
    public ColorPieces Colorpieces
    {
	    get { return colorpieces; }
    }

	private ClearPieces clearpieces;

	public ClearPieces Clearpieces
	{
		get { return clearpieces; }
	}

	private void Awake()
	{
		movablePiece = this.GetComponent<MovablePiece>();
		colorpieces = this.GetComponent<ColorPieces>();
		clearpieces = this.GetComponent<ClearPieces>();
	}

	public void Init(int _x,int _y,Grid.PieceType _pieceType,Grid _grid)
	{
		x = _x;
		y = _y;
		pieceType = _pieceType;
		grid = _grid;
	}

	public bool IsMovePiece()
	{
		return movablePiece != null;
	}

	public bool IsColorpiece()
	{
		return colorpieces != null;
	}

	public bool ISClearable()
	{
		return clearpieces != null;
	}

	void OnMouseEnter()
	{
        grid.EnterPiece(this);
		this.transform.Find("piece").GetComponent<SpriteRenderer>().material.color = Color.gray;
	}



	private void OnMouseExit()
	{

		this.transform.Find("piece").GetComponent<SpriteRenderer>().material.color = Color.white;
	}

 
	void OnMouseDown()
	{
        grid.PressPiece(this);
		copySelfSprite = grid.GetCopySprite(this);
		copySelfSprite.transform.position = grid.GetMousePosition();
	}



	private void OnMouseDrag()
	{
        copySelfSprite.transform.position = grid.GetMousePosition();
	}


	void OnMouseUp()
	{
		grid.ReleasePiece();
		Destroy(copySelfSprite.transform.gameObject);
	}

}

