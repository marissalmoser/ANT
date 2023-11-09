using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager Instance;

    public GameObject Title;
    public GameObject Level;
    public GameObject Challenge;
    public GameObject Boss;

    private bool title;
    private bool level;
    private bool challenge;
    private bool boss;

    public static Action NewLevelTriggered;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        NewLevelTriggered += ChangeMusic;

        Title.SetActive(true);
        title = true;
    }

    public void ChangeMusic()
    {
        switch(SceneManager.GetActiveScene().buildIndex + 1)
        {
            case 0:
                if(!title)
                {
                    PlayTitle();
                }
                break;
            case 1:
            case 2:
            case 3:
                if (!level)
                {
                    PlayLevel();
                }
                break;
            case 4:
                if (!challenge)
                {
                    PlayChallenge();
                }
                break;
            case 5:
                if (!boss)
                {
                    PlayBoss();
                }
                break;
            default:
                if (!title)
                {
                    PlayTitle();
                }
                break;
        }
    }

    private void PlayTitle()
    {
        StartCoroutine(FadeMusic(Title));

        title = true;
        level = false;
        challenge = false;
        boss = false;

        //Title.SetActive(true);
    }
    private void PlayLevel()
    {
        StartCoroutine(FadeMusic(Level));

        title = false;
        level = true;
        challenge = false;
        boss = false;

        //Level.SetActive(true);
    }
    private void PlayChallenge()
    {
        StartCoroutine(FadeMusic(Challenge));

        title = false;
        level = false;
        challenge = true;
        boss = false;

        //Challenge.SetActive(true);
    }
    private void PlayBoss()
    {
        StartCoroutine(FadeMusic(Boss));

        title = false;
        level = false;
        challenge = false;
        boss = true;

        //Boss.SetActive(true);
    }
    IEnumerator FadeMusic(GameObject newMusic)
    {
        print("fading");
        for (float i = 1; i >= 0; i -= 0.03f)
        {
            Title.GetComponent<AudioSource>().volume = i;
            Level.GetComponent<AudioSource>().volume = i;
            Challenge.GetComponent<AudioSource>().volume = i;
            Boss.GetComponent<AudioSource>().volume = i;
            yield return null;
        }
        Title.GetComponent<AudioSource>().Stop();
        Level.GetComponent<AudioSource>().Stop();
        Challenge.GetComponent<AudioSource>().Stop();
        Boss.GetComponent<AudioSource>().Stop();

        newMusic.GetComponent<AudioSource>().volume = 0.5f;
        newMusic.SetActive(true);
    }

    private void OnDestroy()
    {
        NewLevelTriggered -= ChangeMusic;
    }
}
