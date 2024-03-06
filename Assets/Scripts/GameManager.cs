using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;


public class GameManager : MonoBehaviour
{
    [SerializeField] private Deck deck;
    [SerializeField] private Player[] players;
    [SerializeField] private Transform pile;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private TMPro.TMP_Text tip;
    [SerializeField] private GameObject btnDraw;
    [SerializeField] private GameObject btnPass;



    private Card pileTopCard;
    private int playerIndex = 0;
    private bool isClockwise = true;


    private void Start()
    {
        Application.targetFrameRate = 60;
        PlayGame();
    }

    public async void PlayGame()
    {
        gameOver.SetActive(false);
        btnDraw.SetActive(false);
        for (int i = 0; i < pile.childCount; i++)
        {
            Destroy(pile.GetChild(i).gameObject);
        }
        foreach (var player in players)
        {
            player.ResetHand();
            player.showOneCard(false);
        }
        deck.Shuffle();
        playerIndex = 0;
        isClockwise = true;
        tip.text = "";
        await deck.Deal();
        players[0].handInteractable(true);
    }

    public void PlayCard(Card card)
    {
        // tip.SetActive(false);
        if (!pileTopCard || card.isFuncCard || card.CardSuit == pileTopCard.CardSuit || card.CardVlaue == pileTopCard.CardVlaue)
        {
            players[0].handInteractable(false);
            card.transform.SetParent(GameObject.Find("Canvas").transform);
            if (playerIndex != 0) card.GetComponent<Image>().sprite = card.CardSprite;
            DOTween.Sequence()
            .Append(card.transform.DOMove(pile.position, 0.5f)).SetEase(Ease.OutExpo)
            .AppendCallback(() =>
            {
                pileTopCard = card;
                card.transform.SetParent(pile);
                switch (card.CardVlaue)
                {
                    case 11:
                        tip.text = "Reverse the order.";
                        isClockwise = !isClockwise;
                        TurnToNext();
                        break;
                    case 12:
                        tip.text = "Skip the next player.";
                        CalculateNext();
                        TurnToNext();
                        break;
                    case 13:
                        tip.text = "Next player draws two cards.";
                        CalculateNext();
                        DOTween.Sequence()
                        .AppendCallback(() =>
                        {
                            deck.Draw(players[playerIndex].transform);
                        })
                        .AppendInterval(0.5f)
                        .AppendCallback(() =>
                        {
                            deck.Draw(players[playerIndex].transform);
                            TurnToNext();
                        });
                        break;
                    default:
                        tip.text = "";
                        TurnToNext();
                        break;
                }
            });
        }
    }

    public void CalculateNext()
    {
        playerIndex = isClockwise ? ++playerIndex : --playerIndex;
        if (playerIndex < 0) playerIndex = 2;
        if (playerIndex > 2) playerIndex = 0;
    }

    public void TurnToNext()
    {
        btnPass.SetActive(false);
        btnDraw.SetActive(false);
        foreach (var player in players)
        {
            player.showOneCard(player.transform.childCount == 1);
            if (player.transform.childCount == 0)
            {
                GameOver();
                return;
            }
        }
        CalculateNext();
        Player currentPlayer = players[playerIndex];
        List<Card> cards = new List<Card>();
        for (int i = 0; i < currentPlayer.transform.childCount; i++)
        {
            Card card = currentPlayer.transform.GetChild(i).GetComponent<Card>();
            if (card.CardSuit == pileTopCard.CardSuit || card.CardVlaue == pileTopCard.CardVlaue || card.CardVlaue > 10)
                cards.Add(currentPlayer.transform.GetChild(i).GetComponent<Card>());
        }
        if (cards.Count == 0)
        {
            if (playerIndex != 0)
            {
                DOTween.Sequence()
                .AppendInterval(0.75f)
                .AppendCallback(() =>
                {
                    deck.Draw(currentPlayer.transform);
                })
                .AppendInterval(0.75f)
                .AppendCallback(() =>
                {
                    TurnToNext();
                });
            }
            else
            {
                btnDraw.SetActive(true);
            }
        }
        else
        {
            if (playerIndex == 0)
            {
                currentPlayer.handInteractable(true);
                btnPass.SetActive(true);
                btnDraw.SetActive(true);
            }
            else
            {
                StartCoroutine(Delay.DelayToInvoke(() =>
                {
                    int r = Random.Range(0, 2);
                    if (r != 0)
                        PlayCard(cards[Random.Range(0, cards.Count - 1)]);
                    else
                    {
                        currentPlayer.Pass();
                        TurnToNext();
                    }
                }, 2f));
            }
        }
    }

    public void Draw()
    {
        DOTween.Sequence()
        .AppendCallback(() =>
        {
            deck.Draw(players[0].transform);
            btnDraw.SetActive(false);
            btnPass.SetActive(false);
        })
        .AppendInterval(0.5f)
        .AppendCallback(() =>
        {
            TurnToNext();
        });
    }

    public void GameOver()
    {
        gameOver.SetActive(true);
        List<Transform> rank = new List<Transform>();
        for (int i = 0; i < 3; i++)
        {
            rank.Add(players[i].transform);
        }
        rank.Sort(delegate (Transform player1, Transform player2)
        {
            return player1.childCount.CompareTo(player2.childCount);
        });
        switch (rank.IndexOf(players[0].transform))
        {
            case 0:
                gameOver.transform.Find("Rank").GetComponent<TMPro.TMP_Text>().text = "You're the champion!";
                gameOver.transform.Find("Rank").GetComponent<TMPro.TMP_Text>().color = Color.red;
                break;
            case 1:
                gameOver.transform.Find("Rank").GetComponent<TMPro.TMP_Text>().text = "You're the runner-up.";
                gameOver.transform.Find("Rank").GetComponent<TMPro.TMP_Text>().color = Color.cyan;

                break;
            case 2:
                gameOver.transform.Find("Rank").GetComponent<TMPro.TMP_Text>().text = "You're the third runner-up.";
                gameOver.transform.Find("Rank").GetComponent<TMPro.TMP_Text>().color = Color.green;

                break;
        }
        Invoke("PlayGame", 3f);
    }

}
