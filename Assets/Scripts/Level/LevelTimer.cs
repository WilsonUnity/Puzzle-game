using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimer : Level
{

	public float timeInSeconds;
	public int targetScore;

	private float timer = 0.0f;
	private bool timeOut = false;
	
	private void Awake()
	{
		type = LevelType.TIMER;
		hud.SetLevelType (type);
		hud.SetScore (currentScore);
		hud.SetTarget (targetScore);
		hud.SetRemaining (string.Format ("{0}:{1:00}", timeInSeconds / 60, timeInSeconds % 60));
	}

	private void Update()
	{
		if (!timeOut)
		{
			timer += Time.deltaTime;
			hud.SetRemaining (string.Format ("{0}:{1:00}", (int)Mathf.Max((timeInSeconds - timer) / 60, 0), (int)Mathf.Max((timeInSeconds - timer) % 60, 0)));
			if (timeInSeconds - timer <= 0)
			{
				if (currentScore >= targetScore)
				{
					GameWin();
				}
				else
				{
					GameLose();
				}

				timeOut = true;
			}
			
		}
	}
}
