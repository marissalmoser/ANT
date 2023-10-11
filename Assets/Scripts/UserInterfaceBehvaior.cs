/**********************************************************************************

// File Name :         UserInterfaceBehavior.cs
// Author :            Marissa Moser
// Creation Date :     October 5, 2023
//
// Brief Description : 

**********************************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UserInterfaceBehvaior : MonoBehaviour
{
    [SerializeField] private TMP_Text beeVisionText;
    [SerializeField] private TMP_Text webPlatformText;
    [SerializeField] private GameObject errorMessageText;

    private Coroutine errorCoroutineCache;

    void Awake()
    {
        PlayerController.BeeVisionUI += SwitchHeadUI;
        PlayerController.WebShooterUI += SwitchLegUI;
        PlayerController.ErrorMessage += StartErrorCoroutine;
    }

    private void StartErrorCoroutine()
    {
        if(errorCoroutineCache == null)
        {
            errorCoroutineCache = StartCoroutine(ErrorMesageUI());
        }
        else
        {
            StopCoroutine(errorCoroutineCache);
            errorCoroutineCache = StartCoroutine(ErrorMesageUI());
        }
    }

    public void SwitchHeadUI()
    {
        // on UI
        if(!GameManager.Instance.BaseHead)
        {
            print("on");
            beeVisionText.text = "Bee Vision: On";
        }
        // off UI
        else
        {
            print("off");
            beeVisionText.text = "Bee Vision Off";
        }
    }

    public void SwitchLegUI()
    {
        if (!GameManager.Instance.BaseLeg)
        {

            webPlatformText.text = "Web Platforms: On";
        }
        else
        {

            webPlatformText.text = "Web Platforms: Off";
        }
    }

    public IEnumerator ErrorMesageUI()
    {
        errorMessageText.SetActive(true);

        yield return new WaitForSeconds(1);

        errorMessageText.SetActive(false);
        errorCoroutineCache = null;
    }

    private void OnDestroy()
    {
        PlayerController.BeeVisionUI -= SwitchHeadUI;
        PlayerController.WebShooterUI -= SwitchLegUI;
    }
}
