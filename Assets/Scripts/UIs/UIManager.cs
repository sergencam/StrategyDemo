using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    [SerializeField] private InfoUIController m_infoUIController;
    [SerializeField] private ProductUIController m_productUIController;
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
        m_infoUIController.SetBuildingInfo(buildingType, building);
        m_infoUIController.OpenPanel();
    }

    public void CloseInfoPanel()
    {
        m_infoUIController.ClosePanel();
    }
}