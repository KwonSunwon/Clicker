using UnityEngine;
using System.Collections;
using Steamworks;

public class NewMonoBehaviourScript : MonoBehaviour
{
    void Start()
    {
        if (SteamManager.Initialized)
        {
            string name = SteamFriends.GetPersonaName();
            Debug.Log("Steam Name: " + name);
        }
    }
}