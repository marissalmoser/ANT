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

    private int track = 1;

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

        ChangeMusic();
    }

    public void ChangeMusic()
    {
        switch(SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                if(track != 0)
                {
                    PlayTitle();
                }
                break;
            case 1:
            case 2:
            case 3:
                if (track != 1)
                {
                    PlayLevel();
                }
                break;
            case 4:
                if (track != 2)
                {
                    PlayChallenge();
                }
                break;
            case 5:
                if (track != 3)
                {
                    PlayBoss();
                }
                break;
            default:
                if (track != 0)
                {
                    PlayTitle();
                }
                break;
        }
    }

    private void PlayTitle()
    {
        track = 0;

        Title.GetComponent<AudioSource>().volume = 0.5f;
        Title.SetActive(true);
    }
    private void PlayLevel()
    {
        track = 1;

        Level.GetComponent<AudioSource>().volume = 0.5f;
        Level.SetActive(true);
    }
    private void PlayChallenge()
    {
        track = 2;

        Challenge.GetComponent<AudioSource>().volume = 0.5f;
        Challenge.SetActive(true);
    }
    private void PlayBoss()
    {
        track = 3;

        Boss.GetComponent<AudioSource>().volume = 0.5f;
        Boss.SetActive(true);
    }
    public IEnumerator FadeMusic(bool title)
    {
        int cs = SceneManager.GetActiveScene().buildIndex;
        if (cs == 0 || cs == 3 || cs == 4 || cs == 5 || title)
        {
            for (float i = 0.5f; i >= 0; i -= 0.03f)
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

            Title.SetActive(false);
            Level.SetActive(false);
            Challenge.SetActive(false);
            Boss.SetActive(false);

            ChangeMusic();
        }
    }
}
