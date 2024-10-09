using System;
using AYellowpaper.SerializedCollections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoUIController : MonoBehaviour
{
    [SerializeField] private RectTransform m_panel, m_productsPanel, m_productsPanelContent;
    [SerializeField] private SoldierUIItem m_soldierUIItem;
    [SerializeField] private TextMeshProUGUI m_headerText;
    [SerializeField] private Image m_iconImage;
    
    [SerializedDictionary("Soldier Type", "Icon")]
    public SerializedDictionary<SoldierTypes, Sprite> soldierTypesAndIcons;
    [SerializedDictionary("Building Type", "Icon")]
    public SerializedDictionary<BuildingTypes, Sprite> buildingTypesAndIcons;

    public void SetBuildingInfo(BuildingTypes buildingType, Building building)
    {
        m_headerText.text = buildingType.ToString();
        m_iconImage.sprite = buildingTypesAndIcons[buildingType];
        m_productsPanel.gameObject.SetActive(false);
        if(m_productsPanelContent.transform.childCount>0)
            foreach(Transform child in m_productsPanelContent.transform)
                Destroy(child.gameObject);
        if (buildingType == BuildingTypes.Barracks)
        {
            m_productsPanel.gameObject.SetActive(true);
            foreach (SoldierTypes soldierType in Enum.GetValues(typeof(SoldierTypes)))
            {
                var soldierUIItem = Instantiate(m_soldierUIItem, m_productsPanelContent);
                var barrack = building as Barracks;
                soldierUIItem.SetUI(soldierType, barrack.m_soldierSpawnRefPoint.position);
            }
        }
    }

    public void OpenPanel()
    {
        m_panel.gameObject.SetActive(true);
    }
    
    public void ClosePanel()
    {
        m_panel.gameObject.SetActive(false);
    }
}