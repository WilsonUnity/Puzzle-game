using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorPieces : MonoBehaviour {

	public enum ColorType
	{
		黄色,
		紫色,
		绿色,
		红色,
		粉红,
		蓝色	,
		任意
	}

	public int num;
	
	[System.Serializable]
	public struct ColorSprite
	{
		public ColorType colortype;
		public Sprite sprite;
	}

	public ColorSprite[] colorsprite;
	private Dictionary<ColorType, Sprite> ColorDictionary;

	private SpriteRenderer spriteRenderer;

	private ColorType color;
    public ColorType Color
	{
		get { return color; }
	    set { SetColor(value); }
    }

	public int ColorNumber
	{
		get { return colorsprite.Length; }
	}

	private void Awake()
	{
		ColorDictionary = new Dictionary<ColorType, Sprite>();
		spriteRenderer = transform.Find("piece").GetComponent<SpriteRenderer>();

		for (int i = 0; i < colorsprite.Length; i++)
		{
			if (!ColorDictionary.ContainsKey(colorsprite[i].colortype))
			{
				ColorDictionary.Add(colorsprite[i].colortype,colorsprite[i].sprite);
			}
		}
	}

	public void SetColor(ColorType newColor)
	{
		color = newColor;
		if (ColorDictionary.ContainsKey(newColor))
		{
			spriteRenderer.sprite = ColorDictionary[newColor];
		}
	}




















}
