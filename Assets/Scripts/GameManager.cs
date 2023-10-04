/**********************************************************************************

// File Name :         GameManager.cs
// Author :            Marissa Moser
// Creation Date :     September 18, 2023
//
// Brief Description : 

**********************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager gm;

    public bool BaseHead;
    public bool BaseLeg;

    public List<GameObject> BeeVisionObjects = new List<GameObject>();
    public List<GameObject> WebPlatformList = new List<GameObject>();

    void Start()
    {
        if(gm == null)
        {
            gm = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this);
        }
        BaseHead = true;
        BaseLeg = true; 
    }

    public IEnumerator EndLevel()
    {
        print("Game Over");
        //stop movement
        //all bee scream animation
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
