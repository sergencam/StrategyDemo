using UnityEngine;

public interface ISoldierFactory
{
    Soldier CreateSoldier(SoldierTypes soldierType, Vector3 pos);
}