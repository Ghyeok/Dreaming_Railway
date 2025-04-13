using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


// 환승 로직을 담당하는 매니저
public class TransferManager : SingletonManagers<TransferManager>
{
    public int curTransferCount;
    public int maxTransferCount;
    public int dayCount;

    public StationData currentStation;

    public override void Awake()
    {
        base.Awake();
        curTransferCount = 0;
        dayCount = 1;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DetermineMaxTransferCount();
    }

    void DetermineMaxTransferCount()
    {
        switch (dayCount)
        {
            case 1:
                maxTransferCount = 3;
                break;
            case 2:
                maxTransferCount = 4;
                break;
            case 3:
                maxTransferCount = 5;
                break;
            case 4:
                maxTransferCount = 7;
                break;
            case 5:
                maxTransferCount = 9;
                break;
        }
    }
}
