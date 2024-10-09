using System;
using UnityEngine;

public class ProductUIController : MonoBehaviour
{
    [SerializeField] private BuildingUIItem m_buildingUIItem;
    [SerializeField] private RectTransform m_contentSpawnRect;
        
    private void Awake()
    {
        CreateUI();
    }

    private void CreateUI()
    {
        foreach (BuildingTypes buildingType in Enum.GetValues(typeof(BuildingTypes)))
        {
            var buildingUIItem = Instantiate(m_buildingUIItem, m_contentSpawnRect);
            buildingUIItem.SetUI(buildingType);
        }
    }
}