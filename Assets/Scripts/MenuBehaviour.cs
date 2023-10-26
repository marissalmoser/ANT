using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBehaviour : MonoBehaviour
{
    public void StartLevel1()
    {
        StartCoroutine(GameManager.Instance.NextLevel());
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
