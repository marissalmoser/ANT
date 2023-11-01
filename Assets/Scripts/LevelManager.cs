using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> Bees = new List<GameObject>();
    public List<GameObject> BeeVisionObjects = new List<GameObject>();

    void Awake()
    {
        PlayerController.BeeVision += BeeVisionEnabled;
        Time.timeScale = 1;

        if(GameManager.Instance != null)
        {
            GameManager.Instance.WebPlatformList.Clear();
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


    private void OnDisable()
    {
        PlayerController.BeeVision -= BeeVisionEnabled;
    }
}
