using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RandomOrg.CoreApi;

public class Randomizer : MonoBehaviour
{   
    private RandomOrgClient api;
    private string seed = "918834e5-efd1-49ab-932d-0176c6d18ee9";

    public static Randomizer instance;

    private void Awake()
    {
        if(instance) Destroy(instance);
        instance = this;

        //Sets RandomORG client
        api = RandomOrgClient.GetRandomOrgClient(seed);
    }

    #region Basic functions
    /// <summary>
    /// Return a random integer within the min/max range
    /// </summary>
    public int GetRandomInt(int min, int max) 
    {
        return api.GenerateIntegers(1, min, max)[0];
    }

    /// <summary>
    /// Return an array of n random integer(s) within the min/max range
    /// </summary>
    public int[] GetRandomIntSequence(int min, int max, int n)
    {
        return api.GenerateIntegers(n, min, max);
    }

    /// <summary>
    /// Return a random string of the desired lenght
    /// </summary>
    public string GetRandomString(int lenght, string characters = "abcdefghijklmnopqrstuvwxyz")
    {
        return api.GenerateStrings(1, lenght, characters)[0];
    }

    /// <summary>
    /// Return a random UUID of type Guid
    /// </summary>
    public Guid GetRandomUUID()
    {
        return api.GenerateUUIDs(1)[0];
    }
    #endregion Basic functions
}
