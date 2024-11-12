using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class WeaponStockData
{
    private int initialWeaponNumber;
    private int maxWeaponNumber;
    private int gainWeaponNumber;

    private int currentWeaponNumber;
    public int CurrentWeaponNumber => currentWeaponNumber; //getter

    public WeaponStockData(int initialWeaponNumber, int maxWeaponNumber, int gainWeaponNumber)
    {
        this.initialWeaponNumber = initialWeaponNumber;
        this.maxWeaponNumber = maxWeaponNumber;
        this.gainWeaponNumber = gainWeaponNumber;
        this.currentWeaponNumber = initialWeaponNumber;
    }
    public void InitializeWeaponnumber()
    {
        currentWeaponNumber = initialWeaponNumber;
    }

    public void GainingWeaponNunber()
    {
        currentWeaponNumber += gainWeaponNumber;
        if(currentWeaponNumber > maxWeaponNumber)
        {
            currentWeaponNumber = maxWeaponNumber;
        }
    }

    public void DecrementWeaponNumber()
    {
        if(currentWeaponNumber > 0)
        {
            currentWeaponNumber--;
        }
    }
}