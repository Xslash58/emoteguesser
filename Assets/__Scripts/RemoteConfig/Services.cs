using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;

public class Services : MonoBehaviour
{
    public static Services instance = null;

    public delegate void ReadyHandler();
    public event ReadyHandler Ready;

    private async void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        await InitializeRemoteConfigAsync();
    }

    async Task InitializeRemoteConfigAsync()
    {
        //Try Initializing UnityServices
        try
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }

        Ready?.Invoke();
    }
}