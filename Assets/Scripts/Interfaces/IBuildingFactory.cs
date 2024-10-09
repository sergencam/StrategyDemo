using UnityEngine;

public interface IBuildingFactory
{
    Building CreateBuilding(BuildingTypes buildingTypes);
}