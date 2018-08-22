using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Xml.Schema;
using JetBrains.Annotations;
using NUnit.Framework.Interfaces;
using UnityEngine;
using UnityEngine.UI;

public class Grid : MonoBehaviour
{

	public enum PieceType
	{
		Empty,
		NORMAL,
		COUNT,
		BUBBLE,
		ROW_CLEAR,
		COL_CLEAR,
		ANY
	}

    [System.Serializable]
	public struct Piecefrefab
    {
	    public PieceType pietype;
	    public GameObject piecefrefab;
    }
	
	[System.Serializable]
	public struct PiecePosition
	{
		public PieceType piecetype;
		public int x;
		public int y;
	}
	
	[Header("===== Array and reference =====")]
	public Piecefrefab[] piefrefabs;
	public PiecePosition[] piecepositions;
	private GamePiece[,] pieces;
	public GameObject background;
	public Level level;
	
	private Dictionary<PieceType, GameObject> pieceDictionary;
	
	[Header("===== Other =====")]
	public int xDim;
	public int yDim;
    public float filltime;
	
	private GamePiece pressedPiece;
	
	private GamePiece enteredPiece;
    
	private bool gameover = false;

	 
    


	private void Awake()
	{
		pieceDictionary = new Dictionary<PieceType, GameObject>();
		pieces = new GamePiece[xDim, yDim];

		//将类型结构体数组中的两组数据导入字典
		for (int i = 0; i < piefrefabs.Length; i++)
		{
			if (!pieceDictionary.ContainsKey(piefrefabs[i].pietype))
			{
				pieceDictionary.Add(piefrefabs[i].pietype, piefrefabs[i].piecefrefab);
			}
		}

		for (int x = 0; x < xDim; x++)
		{
			for (int y = 0; y < yDim; y++)
			{
				GameObject backgroundObject = Instantiate(background, GetWorldPosition(x,y), Quaternion.identity) 
					as GameObject;
				backgroundObject.transform.SetParent(this.transform);
			}
		}


		for (int i = 0; i < piecepositions.Length; i++)
		{
			if (piecepositions[i].x >= 0 && piecepositions[i].x < xDim && piecepositions[i].y >= 0 &&
			    piecepositions[i].y < yDim)
			{
				SpawnNewPieces(piecepositions[i].x, piecepositions[i].y, piecepositions[i].piecetype);
			}
		}

		//遍历所有网格，如果当前位置为空，则生成空元素
		for (int x = 0; x < xDim; x++)
		{
			for (int y = 0; y < yDim; y++)
			{
				if (pieces[x, y] == null)
				{
					SpawnNewPieces(x, y, PieceType.Empty);
				}
			}
		}
		
		

		StartCoroutine(fill());
		
	}
	
	public IEnumerator fill()
	{
		bool needReFill = true;
		while (needReFill)
		{
			yield return new WaitForSeconds(filltime);
			while (fillstep())
			{
				yield return new WaitForSeconds(filltime);
			}

			needReFill = ClearAllValidPieces();
		}
		
        OpenCollider();

	}

	public bool fillstep()
	{
		bool IsMoveLow = false;
		
		for (int y = yDim - 2; y >= 0; y--)
		{
			for (int loopx = 0; loopx < xDim; loopx++)
			{
				int x = loopx;
				
				GamePiece piece = pieces[x, y];
				 
				
				if (pieces[x, y].IsMovePiece())
				{
					GamePiece piecebelow = pieces[x, y + 1];
					if (piecebelow.GridPieceType == PieceType.Empty) 
					{
						Destroy(piecebelow.gameObject);
						piece.Movablepiece.Move(x, y + 1,filltime);
						pieces[x, y + 1] = piece;
						SpawnNewPieces(x,y,PieceType.Empty);
						IsMoveLow = true;
					}

					else {
						
						for (int diag = -1; diag <= 1; diag++)
						{
							if (diag != 0)
							{
								int diagX = x + diag;

//								

								if (diagX >= 0 && diagX < xDim)
								{
									GamePiece diagonalPiece = pieces [diagX, y + 1];

									if (diagonalPiece.GridPieceType == PieceType.Empty) 
									{
										bool hasPieceAbove = true;

										for (int aboveY = y; aboveY >= 0; aboveY--)
										{
											GamePiece pieceAbove = pieces [diagX, aboveY];

											if (pieceAbove.IsMovePiece())
											{
												break;
											}
											//气泡障碍物
										    else if(!pieceAbove.IsMovePiece() && pieceAbove.GridPieceType != PieceType.Empty)
											{
												hasPieceAbove = false;
												break;
				 							}
										}

										if (!hasPieceAbove)
										{
											Destroy (diagonalPiece.gameObject);
											piece.Movablepiece.Move (diagX, y + 1, filltime);
											pieces [diagX, y + 1] = piece;
											SpawnNewPieces(x, y, PieceType.Empty);
											IsMoveLow = true;
											break;
										}
									} 
								}
							}
						}
					}
				}
            } 
		}

		for (int x = 0; x < xDim; x++)
		{
			GamePiece piecebelow = pieces[x, 0];
			if (piecebelow.GridPieceType == PieceType.Empty)
			{
				Destroy(piecebelow.gameObject);
				GameObject go = Instantiate(pieceDictionary[PieceType.NORMAL], GetWorldPosition(x, -1),
					Quaternion.identity) as GameObject;
				go.transform.SetParent(transform);
				pieces[x,0] = go.GetComponent<GamePiece>();
				pieces[x,0].Init(x, -1, PieceType.NORMAL, this);
				pieces[x,0].Movablepiece.Move(x, 0, filltime);
				pieces[x,0].Colorpieces.SetColor((ColorPieces.ColorType)Random.Range(0,pieces[x,0].Colorpieces.ColorNumber));
				IsMoveLow = true;
			}
		}

		return IsMoveLow;
	}

    public Vector2 GetWorldPosition(int x, int y)
	{
		return new Vector2(transform.position.x - xDim / 2.0f + x, transform.position.y + yDim / 2.0f - y);
		
	}

	public GamePiece SpawnNewPieces(int x,int y,PieceType type)
	{
        GameObject go = Instantiate(pieceDictionary[type],GetWorldPosition(x,y),Quaternion.identity) as GameObject;
		go.transform.SetParent(transform);
        go.name = "Piece" + "(" + x + "," + y + ")"; 
		pieces[x,y] = go.GetComponent<GamePiece>();
		pieces[x,y].Init(x,y,type,this);
		return pieces[x, y];
	}
	
	public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
	{
		return (piece1.X == piece2.X && (int)Mathf.Abs (piece1.Y - piece2.Y) == 1)
		       || (piece1.Y == piece2.Y && (int)Mathf.Abs (piece1.X - piece2.X) == 1);
	}

	
	
	
	/// <summary>
	/// 互换精灵图的位置
	/// </summary>
	/// <param name="piece1"></param>
	/// <param name="piece2"></param>
	public void SwapPieces(GamePiece piece1, GamePiece piece2)
	{
		if (gameover)
		{
			return;
		}

		if (piece1.IsMovePiece() && piece2.IsMovePiece())
		{
			pieces[piece1.X, piece1.Y] = piece2;
			pieces[piece2.X, piece2.Y] = piece1;


			if (GetSameColorPiece(piece1, piece2.X, piece2.Y) != null ||
			    GetSameColorPiece(piece2, piece1.X, piece1.Y) != null ||
			    piece1.GridPieceType == PieceType.ANY || piece2.GridPieceType == PieceType.ANY)
			{
				int piece1X = piece1.X;
				int piece1Y = piece1.Y;

				piece1.Movablepiece.Move(piece2.X, piece2.Y, filltime);
				piece2.Movablepiece.Move(piece1X, piece1Y, filltime);

				if (piece1.GridPieceType == PieceType.ANY && piece1.ISClearable() && piece2.IsColorpiece())
				{
					ClearColor clearColor = piece1.GetComponent<ClearColor>();
					if (clearColor)
					{
						clearColor.Color = piece2.Colorpieces.Color;
					}

					ClearPiece(piece1.X, piece1.Y);
				}
				
				if (piece2.GridPieceType == PieceType.ANY && piece2.ISClearable() && piece1.IsColorpiece())
				{
					ClearColor clearColor = piece2.GetComponent<ClearColor>();
					if (clearColor)
					{
						clearColor.Color = piece1.Colorpieces.Color;
					}

					ClearPiece(piece2.X, piece2.Y);
				}

				ClearAllValidPieces();

				pressedPiece = null;
				enteredPiece = null;
				StartCoroutine(fill());
				level.OnMove();
			}

			else
			{
				pieces[piece1.X, piece1.Y] = piece1;
				pieces[piece2.X, piece2.Y] = piece2;
			}

		}
	}

	
	
	//获取接收到按下事件的精灵
	public void PressPiece(GamePiece piece)
	{
		pressedPiece = piece;
	}

	//获取接收到进入事件的精灵
	public void EnterPiece(GamePiece piece)
	{
		enteredPiece = piece;
	}

	//鼠标抬起事件触发后交换位置
	public void ReleasePiece()
	{
		 if (IsAdjacent (pressedPiece, enteredPiece)) {
			SwapPieces (pressedPiece, enteredPiece);
		}
	}

	
	
	/// <summary>
	/// 获取鼠标在世界空间下的位置，同时加上一个偏移值
	/// </summary>
	/// <returns></returns>
	public Vector2 GetMousePosition()
	{
		Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		Vector2 toWorldPos = Camera.main.ScreenToWorldPoint(mousePos);
		return toWorldPos + new Vector2(-0.5f, 0.5f);
	}

	
	
	/// <summary>
	/// 生成当前点击piece的复制,用来提示当前选择的sprite
	/// </summary>
	/// <param name="piece"></param>
	/// <returns></returns>
	public SpriteRenderer GetCopySprite(GamePiece piece)
	{
		GameObject go = new GameObject("CopySprite");
		SpriteRenderer copysprite = go.AddComponent<SpriteRenderer>();
		copysprite.sprite = piece.transform.Find("piece").GetComponent<SpriteRenderer>().sprite;
		copysprite.sortingOrder = 2;
		return copysprite;
	}

	
	
	/// <summary>
	/// 在网格填充完毕之后将BoxCollider启用
	/// </summary>
	public void OpenCollider()
	{
		for (int xIndex = 0; xIndex < xDim; xIndex++)
		{
			for (int yIndex = 0; yIndex < yDim; yIndex++)
			{
				if (pieces[xIndex, yIndex].IsMovePiece())
				{
					pieces[xIndex, yIndex].transform.GetComponent<BoxCollider2D>().enabled = true;
				}
			}
		}
    }

	/// <summary>
	/// 检查相邻的sprite是否为同一颜色，为同一颜色就加入匹配列表。否则匹配列表返回NUll。
	/// </summary>
	/// <param name="piece">当前鼠标点击选择的对象</param>
	/// <param name="newX">鼠标进入对象的X</param>
	/// <param name="newY">鼠标进入对象的Y</param>
	/// <returns></returns>
	public List<GamePiece> GetSameColorPiece(GamePiece piece, int newX, int newY)
	{
		if (piece.IsColorpiece())
		{
			ColorPieces.ColorType Color = piece.Colorpieces.Color;
			
			List<GamePiece> horizontalList = new List<GamePiece>();
			List<GamePiece> verticaList = new List<GamePiece>();
			List<GamePiece> matching = new List<GamePiece>();

			horizontalList.Add(piece);

			for (int i = 0; i <= 1; i++)
			{
				for (int Xindex = 1; Xindex < xDim; Xindex++)
				{
					int x;
					if (i == 0)
					{
						x = newX - Xindex;
					}
					else
					{
						x = newX + Xindex;
					}

					if (x < 0 || x >= xDim)
					{
						break;
					}

					if (pieces[x, newY].IsColorpiece() && pieces[x, newY].Colorpieces.Color == Color)
					{
						horizontalList.Add(pieces[x, newY]);
					}
					else
					{
						break;
					}
				}
			}

			if (horizontalList.Count >= 3)
			{
				for (int x = 0; x < horizontalList.Count; x++)
				{
					matching.Add(horizontalList[x]);
				}
			}

			if (horizontalList.Count >= 3) {
				for (int i = 0; i < horizontalList.Count; i++) {
					for (int dir = 0; dir <= 1; dir++) {
						for (int yOffset = 1; yOffset < yDim; yOffset++) {
							int y;

							if (dir == 0) { // Up
								y = newY - yOffset;
							} else { // Down
								y = newY + yOffset;
							}

							if (y < 0 || y >= yDim) {
								break;
							}

							if (pieces [horizontalList [i].X, y].IsColorpiece() && pieces [horizontalList [i].X, y].Colorpieces.Color == Color) {
								verticaList.Add (pieces [horizontalList [i].X, y]);
							} else {
								break;
							}
						}
					}

					if (verticaList.Count < 2) { //至少2个相连
						verticaList.Clear ();
					} else {
						for (int j = 0; j < verticaList.Count; j++) {
							matching.Add (verticaList [j]);
						}

						break;
					}
				}
			}

			if (matching.Count >= 3)
			{
				return matching;
			}
			
			horizontalList.Clear();
			verticaList.Clear();
			
			
			verticaList.Add(piece);

			for (int i = 0; i <= 1; i++)
			{
				for (int Yindex = 1; Yindex < yDim; Yindex++)
				{
					int y;
					if (i == 0)
					{
						y = newY - Yindex;
					}
					else
					{
						y = newY + Yindex;
					}

					if (y < 0 || y >= yDim)
					{
						break;
					}

					if (pieces[newX, y].IsColorpiece() && pieces[newX, y].Colorpieces.Color == Color)
					{
						verticaList.Add(pieces[newX, y]);
					}
					else
					{
						break;
					}
				}
			}

			if (verticaList.Count >= 3)
			{
				for (int x = 0; x < verticaList.Count; x++)
				{
					matching.Add(verticaList[x]);
				}
			}
			
			if (verticaList.Count >= 3) {
				for (int i = 0; i < verticaList.Count; i++) {
					for (int dir = 0; dir <= 1; dir++) {
						for (int xOffset = 1; xOffset < xDim; xOffset++) {
							int x;

							if (dir == 0) { // Left
								x = newX - xOffset;
							} else { // Right
								x = newX + xOffset;
							}

							if (x < 0 || x >= xDim) {
								break;
							}

							if (pieces [x, verticaList[i].Y].IsColorpiece() && pieces [x, verticaList[i].Y].Colorpieces.Color == Color) {
								horizontalList.Add (pieces [x, verticaList[i].Y]);
							} else {
								break;
							}
						}
					}

					if (horizontalList.Count < 2) {//至少2个相连
						horizontalList.Clear ();
					} else {
						for (int j = 0; j < horizontalList.Count; j++) {
							matching.Add (horizontalList [j]);
						}

						break;
					}
				}
			}

			if (matching.Count >= 3)
			{
				return matching;
			}
		}

		return null;

	}

	public bool ClearAllValidPieces()
	{
		bool IsNeedReFill = false;
		for (int y = 0; y < yDim; y++)
		{
			for (int x = 0; x < xDim; x++)
			{
				if (pieces[x, y].ISClearable())
				{
					List<GamePiece> matching = GetSameColorPiece(pieces[x, y], x, y);
					if (matching != null)
					{
						PieceType specialPieceType = PieceType.COUNT;
						GamePiece randomPiece = matching[Random.Range (0, matching.Count)];
						int specialPieceX = randomPiece.X;
						int specialPieceY = randomPiece.Y;
						
						if (matching.Count == 4) 
						{
							if (pressedPiece == null || enteredPiece == null) {
								specialPieceType = (PieceType)Random.Range ((int)PieceType.ROW_CLEAR, (int)PieceType.COL_CLEAR);
							} 
							else if (pressedPiece.GridPieceType != PieceType.ROW_CLEAR &&
							           pressedPiece.Y == enteredPiece.Y) 
							{
								specialPieceType = PieceType.ROW_CLEAR;
							} 
							else if(pressedPiece.GridPieceType != PieceType.COL_CLEAR &&
							          pressedPiece.X == enteredPiece.X)
							{
								specialPieceType = PieceType.COL_CLEAR;
							}
                            else
							{
								specialPieceType = PieceType.COUNT;
							}
						}
						
						//匹配数大于5生成任意颜色都可以消除的特殊物件
						else if (matching.Count >= 5)
						{
							specialPieceType = PieceType.ANY;
						}

						for (int i = 0; i < matching.Count; i++)
						{
							if (ClearPiece(matching[i].X, matching[i].Y))
							{
								IsNeedReFill = true;
								
								if (matching [i] == pressedPiece || matching [i] == enteredPiece) {
									specialPieceX = matching [i].X;
									specialPieceY = matching [i].Y;
								}
							}
						}

						//特殊物件不为Count时执行生成
						if (specialPieceType != PieceType.COUNT)
						{
                            Destroy(pieces[specialPieceX, specialPieceY]);
							GamePiece newPiece = SpawnNewPieces(specialPieceX, specialPieceY, specialPieceType);
							Debug.Log(newPiece.X+""+newPiece.Y);

							if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COL_CLEAR)
							    && newPiece.IsColorpiece() && matching[0].IsColorpiece())
							{
								newPiece.Colorpieces.SetColor(matching[0].Colorpieces.Color);
							}
							
							else if (specialPieceType == PieceType.ANY && newPiece.IsColorpiece())
							{
								newPiece.Colorpieces.SetColor(ColorPieces.ColorType.任意);
							}

						}
					}
				}
			}
		}

		return IsNeedReFill;
	}

	public bool ClearPiece(int x,int y)
	{
		if (pieces[x, y].ISClearable() && !pieces[x, y].Clearpieces.IsBeginAnimation)
		{
			pieces[x,y].Clearpieces.Clear();
			SpawnNewPieces(x, y, PieceType.Empty);
			ClearBubble(x, y);
			
			return true;
		}

		return false;
	}

	public void ClearBubble(int x, int y)
	{
		for (int IndexX = x - 1; IndexX <= x + 1; IndexX++)
		{
			if (IndexX != 0 && IndexX >= 0 && IndexX < xDim)
			{
				if (pieces[IndexX, y].GridPieceType == PieceType.BUBBLE && pieces[IndexX, y].ISClearable())
				{
					pieces[IndexX,y].Clearpieces.Clear();
					SpawnNewPieces(IndexX, y, PieceType.Empty);
				}
			}
		}
		
		for (int IndexY = y - 1; IndexY <= y + 1; IndexY++)
		{
			if (IndexY != 0 && IndexY >= 0 && IndexY < yDim)
			{
				if (pieces[x, IndexY].GridPieceType == PieceType.BUBBLE && pieces[x, IndexY].ISClearable())
				{
					pieces[x,IndexY].Clearpieces.Clear();
					SpawnNewPieces(x, IndexY, PieceType.Empty);
				}
			}
		}
	}

	public void ClearRow(int col)
	{
		for (int x = 0; x < xDim; x++)
		{
			ClearPiece(x, col);
		}
	}

	public void ClearCol(int row)
	{
		for (int y = 0; y < yDim; y++)
		{
			ClearPiece(row, y);
		}
	}

	public void ClearColor(ColorPieces.ColorType color)
	{
		for (int x = 0; x < xDim; x++)
		{
			for (int y = 0; y < yDim; y++)
			{
				if (pieces[x, y].IsColorpiece() &&
				    (pieces[x, y].Colorpieces.Color == color || color == ColorPieces.ColorType.任意))
				{
					ClearPiece(x, y);
				}
			}
		}
	}

	//让游戏结束
	public void GameOver()
	{
		gameover = true;
	}

	public List<GamePiece> GetGamePieceCount(PieceType type)
	{
		List<GamePiece> gamePieceList = new List<GamePiece>();
		for (int x = 0; x < xDim; x++)
		{
			for (int y = 0; y < yDim; y++)
			{
				if (pieces[x, y].GridPieceType == type)
				{
					gamePieceList.Add(pieces[x, y]);
				}
			}
		}

		return gamePieceList;
	}




}
