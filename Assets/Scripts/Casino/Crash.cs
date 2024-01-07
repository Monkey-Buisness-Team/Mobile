using System.Collections;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class Crash : MonoBehaviour
{
    [SerializeField] private float houseEdge = 2;
    private float currentCrashMultiplier;

    [SerializeField] TextMeshProUGUI currentMultiplierText;
    [SerializeField] CrashPreviousMultiplier previousCrashPrefab;
    [SerializeField] Transform crashHistoryContainer;
    Queue<CrashPreviousMultiplier> crashesHistory = new Queue<CrashPreviousMultiplier>();

    void OnEnable()
    {
        //TODO : Load from database the previous crashes here and SetMultipliers of each previous crash
        for(int i = 0; i < 6; i++)
        {
            CrashPreviousMultiplier crash = Instantiate(previousCrashPrefab, crashHistoryContainer);
            crashesHistory.Enqueue(crash);
        }
        
    }

    public void StartCrashGame()
    {
        GenerateCrashMultiplier();
        currentMultiplierText.text = $"x{currentCrashMultiplier}";
    }

    public void CrashGame()
    {   
        CrashPreviousMultiplier crash = crashesHistory.Dequeue();
        crash.SetMultiplier(currentCrashMultiplier);
        crash.transform.SetSiblingIndex(0);
        crashesHistory.Enqueue(crash);
        currentMultiplierText.text = "BOOM";
    }

    void GenerateCrashMultiplier()
    {
        ulong e = (ulong)Math.Pow(2, 32);

        // Generating a random 32-bit unsigned integer
        ulong h = GetRandomUInt32();

        // If h % (100 / house edge) is 0 makes the game crash at x1
        if (h % (100 / houseEdge) == 0)
        {
            currentCrashMultiplier = 1.00f;
        }

        float r = (float)(Math.Floor((100.0 * e - h) / (e - h)) / 100.0);

        currentCrashMultiplier = r;
    }

    ulong GetRandomUInt32()
    {
        byte[] buffer = BitConverter.GetBytes(RandomNumberGenerator.GetInt32(0, 2147483647));
        if (BitConverter.IsLittleEndian)
            Array.Reverse(buffer);

        return BitConverter.ToUInt32(buffer, 0);
    }
    void Update()
    {
        
    }

}


