using UnityEngine;
using System.Collections;

public class LevelObStacle : Level {

    
    public int numMoves;
    //获取障碍物类型
    public Grid.PieceType[] obstacleTypes;

    private int movesUsed = 0;
    private int numObstaclesLeft;

    // Use this for initialization
    void Start () {
        type = LevelType.OBSTACLE;

        //累计障碍物总数
        for (int i = 0; i < obstacleTypes.Length; i++) {
            numObstaclesLeft += grid.GetGamePieceCount(obstacleTypes [i]).Count;
        }
        
        hud.SetLevelType (type);
        hud.SetScore (currentScore);
        hud.SetTarget (numObstaclesLeft);
        hud.SetRemaining (numMoves);
    }
	
    

    public override void OnMove()
    {
        movesUsed++;

        hud.SetRemaining (numMoves - movesUsed);

        //步数为零障碍物仍存在，判定游戏失败
        if (numMoves - movesUsed == 0 && numObstaclesLeft > 0) {
            GameLose ();
        }
    }

    public override void OnPieceCleared(GamePiece piece)
    {
        base.OnPieceCleared (piece);

        for (int i = 0; i < obstacleTypes.Length; i++) {
            //当前消除掉的piece是属于障碍类型
            if (obstacleTypes [i] == piece.GridPieceType) {
                numObstaclesLeft--;
                hud.SetTarget (numObstaclesLeft);
                //障碍物全部消除完毕的时候游戏胜利并计算总分
                if (numObstaclesLeft == 0) {
                    currentScore += 1000 * (numMoves - movesUsed);
                    hud.SetScore (currentScore);
                    GameWin ();
                }
            }
        }
    }
}
