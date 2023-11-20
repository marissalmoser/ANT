/**********************************************************************************

// File Name :         BeeStates.cs
// Author :            Marissa Moser
// Creation Date :     September 24, 2023
//
// Brief Description : This script manages the behaviors that reset for each level.
This includes clearing the web platform list, bee vision objects.

**********************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> Bees = new List<GameObject>();
    public List<GameObject> BeeVisionObjects = new List<GameObject>();
    public static bool IsCaught;

    [SerializeField] private GameObject BKMM;

    void Awake()
    {
        PlayerController.BeeVision += BeeVisionEnabled;
        Time.timeScale = 1;
        IsCaught = false;

        if(GameManager.Instance != null)
        {
            GameManager.Instance.WebPlatformList.Clear();
        }

        if (BackgroundMusicManager.Instance == null)
        {
            Instantiate(BKMM, transform.position, transform.rotation);
        }
    }

    private void BeeVisionEnabled()
    {
        if(!GameManager.Instance.BaseHead)
        {
            foreach (var vision in BeeVisionObjects)
            {
                vision.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        else
        {
            foreach (var vision in BeeVisionObjects)
            {
                vision.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public void GotCaught()
    {
        IsCaught = true;
    }

    public void Escaped()
    {
        IsCaught = false;
    }

    private void OnDisable()
    {
        PlayerController.BeeVision -= BeeVisionEnabled;
    }
}
