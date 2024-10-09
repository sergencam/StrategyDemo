using UnityEngine;

public class FactoryManager : MonoBehaviour
{
    public static FactoryManager Instance;
    private ProductManager m_productManager;
    [SerializeField] private SoldierFactory m_soldierFactory;
    [SerializeField] private BuildingFactory m_buildingFactory;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        m_productManager = ProductManager.Instance;
    }

    public void CreateSoldier(SoldierTypes soldierType, Vector3 pos)
    {
        var createdSoldier = m_soldierFactory.CreateSoldier(soldierType, pos);
        createdSoldier.OnSelect();
        m_productManager.LastSelectedSoldier = createdSoldier;
    }
    
    public void CreateBuilding(BuildingTypes buildingType)
    {
        m_buildingFactory.CreateBuilding(buildingType);
    }

    public void AddBuildingToPool(Building building)
    {
        m_buildingFactory.AddBuildingToPool(building);
    }
}