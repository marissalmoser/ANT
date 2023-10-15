using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> Bees = new List<GameObject>();
    public List<GameObject> BeeVisionObjects = new List<GameObject>();

    void Start()
    {
        PlayerController.BeeVision += BeeVisionEnabled;
    }
    private void BeeVisionEnabled()
    {
        if(!GameManager.Instance.BaseHead)
        {
            foreach (var vision in BeeVisionObjects)
            {
                //check if var is not null?
                print("bes");
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
