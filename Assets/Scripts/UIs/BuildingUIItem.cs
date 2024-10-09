using UnityEngine.UI;

public class BuildingUIItem : UIItem
{
    public void SetUI(BuildingTypes buildingType)
    {
        m_nameText.text = buildingType.ToString();
        GetComponent<Button>().onClick.AddListener(()=>OnClick(buildingType));
    }
    
    private void OnClick(BuildingTypes buildingType)
    {
        if (UIManager.Instance.IsProductSelected) return;
        FactoryManager.Instance.CreateBuilding(buildingType);
        UIManager.Instance.IsProductSelected = true;
    }
}