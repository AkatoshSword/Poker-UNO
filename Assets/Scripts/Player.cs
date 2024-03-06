using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    [SerializeField] private GameObject oneCard;
    [SerializeField] private GameObject pass;


    public void showOneCard(bool b)
    {
        oneCard.SetActive(b);
    }

    public void Pass()
    {
        pass.SetActive(true);
        StartCoroutine(Delay.DelayToInvoke(() =>
        {
            pass.SetActive(false);
        }, 1f));
    }

    public void ResetHand()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(transform.GetChild(i).gameObject);
        }
    }

    public void handInteractable(bool b)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).GetComponent<Button>().interactable = b;
        }
    }
}
