using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private InfoUIController m_infoUIController;
    [SerializeField] private ProductUIController m_productUIController;
    private Building m_shownBuilding;
    private bool m_isProductSelected;
    public bool IsProductSelected
    {
        get => m_isProductSelected;
        set => m_isProductSelected = value;
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

    public void OpenInfoPanel(BuildingTypes buildingType, Building building)
    {
        if(m_shownBuilding!= null && building!= m_shownBuilding)
            m_shownBuilding.SetOutline(false);
        m_shownBuilding = building;
        m_infoUIController.SetBuildingInfo(buildingType, building);
        m_infoUIController.OpenPanel();
    }

    public void CloseInfoPanel()
    {
        if(m_shownBuilding!= null)
            m_shownBuilding.SetOutline(false);
        m_shownBuilding = null;
        m_infoUIController.ClosePanel();
    }
    
    public void CheckCanCloseInfoPanel(Building building)
    {
        if(m_shownBuilding== null || building != m_shownBuilding)return;
        CloseInfoPanel();
    }
}