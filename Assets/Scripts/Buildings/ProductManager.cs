using UnityEngine;

public class ProductManager : MonoBehaviour
{
    public static ProductManager Instance;
    private Soldier m_lastSelectedSoldier;

    public Soldier LastSelectedSoldier
    {
        get => m_lastSelectedSoldier;
        set => m_lastSelectedSoldier = value;
    }
    
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

    public void OnBuildingSelectedForAttack(Building building)
    {
        if (m_lastSelectedSoldier != null)
            m_lastSelectedSoldier.OnAttackToBuilding(building);
    }

}