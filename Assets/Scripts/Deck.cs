using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.U2D;

public class Deck : MonoBehaviour
{
    [SerializeField] private Card prefab;
    [SerializeField] private Transform[] players;
    [SerializeField] private int cardIndex = 0;
    [SerializeField] private SpriteAtlas spriteAtlas;
    [SerializeField] private Sprite[] cardSprites;

    private void Awake()
    {
        cardSprites = new Sprite[spriteAtlas.spriteCount];
        spriteAtlas.GetSprites(cardSprites);
    }

    public void Shuffle()
    {
        gameObject.SetActive(true);
        cardIndex = 0;
        for (int i = 0; i < cardSprites.Length; i++)
        {
            int k = UnityEngine.Random.Range(0, cardSprites.Length - 1);
            Sprite temp = cardSprites[i];
            cardSprites[i] = cardSprites[k];
            cardSprites[k] = temp;
        }
    }

    public async Task Deal()
    {
        while (cardIndex < 21)
        {
            Transform target = players[cardIndex % 3];
            Draw(target);
            if (cardIndex % 3 == 0)
            {
                await Task.Delay(300);
            }
            else
            {
                await Task.Delay(100);
            }
        }
    }

    public void Draw(Transform target)
    {
        if (cardIndex == cardSprites.Length) Shuffle();
        if (cardIndex == cardSprites.Length - 1) gameObject.SetActive(false);
        foreach (Transform player in players)
        {
            if (player.Find(cardSprites[cardIndex].name) != null)
            {
                cardIndex++;
                Draw(target);
                return;
            }
        }
        Card card = Instantiate(prefab) as Card;
        card.initCard(cardSprites[cardIndex]);
        card.transform.SetParent(transform.parent);
        card.transform.position = transform.position;
        card.transform.localScale = transform.localScale;
        Vector3 dest;
        if (target == players[0])
            dest = target.childCount == 0 ? target.position : target.GetChild(target.childCount - 1).position + new Vector3(40f, 0, 0);
        else
            dest = target.childCount == 0 ? target.position : target.GetChild(target.childCount - 1).position - new Vector3(0, 20f, 0);
        DOTween.Sequence()
        .Append(card.transform.DOMove(dest, 0.3f).SetEase(Ease.OutExpo))
        .AppendCallback(() =>
        {
            card.transform.SetParent(target);
            if (target == players[0]) card.ShowSprite();
        });
        cardIndex++;
    }
}
