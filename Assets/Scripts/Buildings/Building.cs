using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Building : MonoBehaviour
{
    [SerializeField]private float m_hp;
    private float m_startHp;
    [SerializeField] private TextMeshProUGUI m_hpText;
    [SerializeField] private BuildingTypes m_buildingType;
    [SerializeField] private GameObject m_outline, m_damageFlash;
    private bool m_isPlaced;
    private SpriteRenderer m_spriteRenderer;
    private TileManager m_tileManager;
    private UIManager m_uiManager;
    private Tile m_activeTile;
    private Collider2D m_collider;
    private Coroutine m_damageFlashCoroutine;

    private void Start()
    {
        m_tileManager = TileManager.Instance;
        m_uiManager = UIManager.Instance;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<Collider2D>();
        transform.GetChild(0).gameObject.SetActive(false);
        m_startHp = m_hp;
        m_collider.enabled = false;
        m_hpText.text = "HP:" + m_hp;
        SetOpacity(0.5f);
    }

    private void Update()
    {
        CheckClickActions();
    }

    private void LateUpdate()
    {
        SetPositionToMousePos();
    }

    private void SetPositionToMousePos()
    {
        if(m_isPlaced) return;

        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            bool isPlaceAreaEmpty = PlaceObjectOnGrid(false);
            if (isPlaceAreaEmpty == false)
                return;
            m_collider.enabled = true;
            transform.GetChild(0).gameObject.SetActive(true);
            m_isPlaced = true;
            m_uiManager.IsProductSelected = false;
            SetOutline(true);
            m_uiManager.OpenInfoPanel(m_buildingType, this);
            SetOpacity(1f);
            return;
        }
        PlaceObjectOnGrid(true);
    }
    
    private bool PlaceObjectOnGrid(bool highlight)
    {
        if(!m_activeTile)
            m_activeTile = m_tileManager.ActiveTile;
        else if(m_activeTile == m_tileManager.ActiveTile && highlight)
            return false;
        m_activeTile = m_tileManager.ActiveTile;
        Vector3 gridPosition = m_activeTile.transform.position;
        Vector3 realWorldSize = GetRealWorldSize();
        gridPosition.z = 0;
        gridPosition.x += realWorldSize.x / 2f - m_tileManager.tileSize/2f;
        gridPosition.y += realWorldSize.y / 2f - m_tileManager.tileSize/2f;
        transform.position = gridPosition;

        bool isPlaceAreaEmpty = true;
        int tileLayer = LayerMask.NameToLayer("Tile");
        int layerMask = ~(1 << tileLayer); 
        Collider2D[] collidersAtPosition = Physics2D.OverlapBoxAll(gridPosition, realWorldSize, 0f, layerMask);
        if (collidersAtPosition.Length > 0)
            isPlaceAreaEmpty = false;
        
        if(highlight || isPlaceAreaEmpty == false)
            m_tileManager.HighlightCoveredTiles(gridPosition, realWorldSize, isPlaceAreaEmpty?Color.green:Color.red);
        else
            m_tileManager.DisableCoveredTiles(gridPosition, realWorldSize);
        return isPlaceAreaEmpty;
    }

    private Vector2 GetRealWorldSize()
    {
        Vector2 spriteSize = m_spriteRenderer.sprite.rect.size;
        float pixelToUnits = m_spriteRenderer.sprite.pixelsPerUnit;
        return spriteSize / pixelToUnits;
    }

    private void SetOpacity(float opacity)
    {
        var srColor = m_spriteRenderer.color;
        srColor.a = opacity;
        m_spriteRenderer.color = srColor;
    }

    private void CheckClickActions()
    {
        if(!m_isPlaced)return;
        if (Input.GetMouseButtonDown(1) && CheckObjectUnderMouse())
            ProductManager.Instance.OnBuildingSelectedForAttack(this);
        
        if (Input.GetMouseButtonDown(0) && CheckObjectUnderMouse())
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SetOutline(true);
                m_uiManager.OpenInfoPanel(m_buildingType, this);
            }
    }
    
    private bool CheckObjectUnderMouse()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        if (hit.collider)
        {
            GameObject clickedObject = hit.collider.gameObject;
            if (clickedObject == gameObject)
                return true;
        }
        return false;
    }

    private IEnumerator DamageFlashRoutine(float flashTime)
    {
        m_damageFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        m_damageFlash.gameObject.SetActive(false);
    }
    
    public void OnTakeDamage(float damage)
    {
        m_hp -= damage;
        m_hpText.text = "HP:" + m_hp;
        if(m_damageFlashCoroutine!=null)
            StopCoroutine(m_damageFlashCoroutine);
        if(m_hp<=0)
            OnDead();
        else
            m_damageFlashCoroutine = StartCoroutine(DamageFlashRoutine(0.1f));
    }

    private void OnDead()
    {
        m_tileManager.EnableCoveredTiles(transform.position, GetRealWorldSize());
        gameObject.SetActive(false);
        FactoryManager.Instance.AddBuildingToPool(this);
        m_uiManager.CheckCanCloseInfoPanel(this);
    }

    public Building OnSelectedFromPool()
    {
        m_hp = m_startHp;
        m_hpText.text = "HP:" + m_hp;
        m_collider.enabled = false;
        m_isPlaced = false;
        SetOpacity(0.5f);
        transform.GetChild(0).gameObject.SetActive(false);
        gameObject.SetActive(true);
        m_damageFlash.SetActive(false);
        return this;
        
    }
    
    public void SetOutline(bool active)
    {
        m_outline.SetActive(active);
    }
}