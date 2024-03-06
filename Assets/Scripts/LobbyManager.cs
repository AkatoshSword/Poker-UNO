using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public GameObject[] dialogs;

    void Start()
    {
        foreach (var dialog in dialogs)
        {
            dialog.SetActive(false);
        }
    }

    public void OpenDialog(GameObject dialog)
    {
        dialog.SetActive(true);
    }

    public void CloseDialog(GameObject dialog)
    {
        dialog.SetActive(false);
    }

}
