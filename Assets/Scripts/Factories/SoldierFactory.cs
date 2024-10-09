using System.Collections.Generic;
using UnityEngine;

public class SoldierFactory : MonoBehaviour, ISoldierFactory
{
    [SerializeField] private Soldier1 m_soldier1;
    [SerializeField] private Soldier2 m_soldier2;
    [SerializeField] private Soldier3 m_soldier3;

    private List<Soldier> m_createdSoldiers = new();

    //Creates soldier from scratch and sets position
    public Soldier CreateSoldier(SoldierTypes soldierType, Vector3 pos)
    {
        Soldier createdSoldier;
        switch (soldierType)
        {
            case SoldierTypes.Soldier1:
                createdSoldier = Instantiate(m_soldier1,pos,Quaternion.identity);
                break;
            case SoldierTypes.Soldier2:
                createdSoldier = Instantiate(m_soldier2,pos,Quaternion.identity);
                break;
            case SoldierTypes.Soldier3:
                createdSoldier = Instantiate(m_soldier3,pos,Quaternion.identity);
                break;
            default:
                Debug.LogError("FAILED TO FIND SOLDIER TYPE!");
                return null;
        }
        
        m_createdSoldiers.Add(createdSoldier);
        return createdSoldier;
    }
}