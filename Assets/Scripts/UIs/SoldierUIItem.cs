using UnityEngine;
using UnityEngine.UI;

public class SoldierUIItem : UIItem
{
    public void SetUI(SoldierTypes soldierType, Vector3 soldierSpawnPos)
    {
        m_nameText.text = soldierType.ToString();
        GetComponent<Button>().onClick.AddListener(()=>OnClick(soldierType, soldierSpawnPos));
    }
    
    private void OnClick(SoldierTypes soldierType, Vector3 soldierSpawnPos)
    {
        FactoryManager.Instance.CreateSoldier(soldierType, soldierSpawnPos);
    }
}