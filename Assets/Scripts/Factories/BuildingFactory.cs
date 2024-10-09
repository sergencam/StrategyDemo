using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BuildingFactory : MonoBehaviour, IBuildingFactory
{
    [SerializeField] private Barracks m_barracks;
    [SerializeField] private Powerplant m_powerplant;

    private List<Building> m_createdBuildings = new();
    private List<Building> m_deactivatedBuildingsPool = new();

    public Building CreateBuilding(BuildingTypes buildingTypes)
    {
        Building createdBuilding;
        if (m_deactivatedBuildingsPool.Count > 0)
        {
            switch (buildingTypes)
            {
                case BuildingTypes.Barracks:
                    if (m_deactivatedBuildingsPool.OfType<Barracks>().Any())
                    {
                        createdBuilding = m_deactivatedBuildingsPool.OfType<Barracks>().ToList()[0].OnSelectedFromPool();
                        m_deactivatedBuildingsPool.Remove(createdBuilding);
                        return createdBuilding;
                    }
                    break;
                case BuildingTypes.Powerplant:
                    if (m_deactivatedBuildingsPool.OfType<Powerplant>().Any())
                    {
                        createdBuilding = m_deactivatedBuildingsPool.OfType<Powerplant>().ToList()[0].OnSelectedFromPool();
                        m_deactivatedBuildingsPool.Remove(createdBuilding);
                        return createdBuilding;
                    }
                    break;
                default:
                    Debug.LogError("FAILED TO FIND BUILDING TYPE!");
                    return null;
            }
        }
        
        switch (buildingTypes)
        {
            case BuildingTypes.Barracks:
                createdBuilding = Instantiate(m_barracks,Vector3.zero,Quaternion.identity);
                break;
            case BuildingTypes.Powerplant:
                createdBuilding = Instantiate(m_powerplant,Vector3.zero,Quaternion.identity);
                break;
            default:
                Debug.LogError("FAILED TO FIND BUILDING TYPE!");
                return null;
        }
        m_createdBuildings.Add(createdBuilding);
        return createdBuilding;
    }

    public void AddBuildingToPool(Building building)
    {
        if(!m_deactivatedBuildingsPool.Contains(building))
            m_deactivatedBuildingsPool.Add(building);
    }
}