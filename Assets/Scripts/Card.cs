using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class Card : MonoBehaviour
{
    public int CardVlaue { get; set; }
    public char CardSuit { get; set; }
    public Sprite CardSprite { get; set; }

    public bool isFuncCard = false;

    public void initCard(Sprite sprite)
    {
        CardSprite = sprite;
        CardSuit = sprite.name.ToCharArray()[0];
        CardVlaue = int.Parse(sprite.name.Substring(1, sprite.name.Length - 8));
        name = sprite.name;
        isFuncCard = CardVlaue > 10;
    }

    public void ShowSprite()
    {
        DOTween.Sequence()
        .Append(transform.DORotate(new Vector3(0f, 90f, 0f), 0.1f))
        .AppendCallback(() =>
        {
            gameObject.GetComponent<Image>().sprite = CardSprite;
            // transform.localScale = new Vector3(1.05f, 1.05f, 1f);
        })
        .Append(transform.DORotate(new Vector3(0f, 0f, 0f), 0.1f));
    }

    public void onCardClick()
    {
        FindObjectOfType<GameManager>().PlayCard(this);
    }
}
