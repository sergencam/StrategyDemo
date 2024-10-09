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
        //Checks for attacks and open info panel
        CheckClickActions();
    }

    private void LateUpdate()
    {
        //Setting position to mouse position when not place to grid
        SetPositionToMousePos();
    }

    private void SetPositionToMousePos()
    {
        if(m_isPlaced) return;
        //If left click is performed and a ui is not clicked or a filled place is not clicked, the object will be placed on the tiles
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
        //Highlighting covered grids
        PlaceObjectOnGrid(true);
    }
    
    private bool PlaceObjectOnGrid(bool highlight)
    {
        //Sets building position to active tile position and highlights the covered tiles
        //If highlight bool is false and place area empty it will set disable covered tiles
        
        //Checks for is last active tile same as before and if it is then return
        if(m_activeTile == m_tileManager.ActiveTile && highlight)
            return false;
        m_activeTile = m_tileManager.ActiveTile;
        
        Vector3 gridPosition = m_activeTile.transform.position;
        Vector3 realWorldSize = GetRealWorldSize();
        gridPosition.z = 0;
        gridPosition.x += realWorldSize.x / 2f - m_tileManager.tileSize/2f;
        gridPosition.y += realWorldSize.y / 2f - m_tileManager.tileSize/2f;
        transform.position = gridPosition;

        //Detects is place area empty with throw raycast
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

    //Gets real world size by sprite size
    private Vector2 GetRealWorldSize()
    {
        Vector2 spriteSize = m_spriteRenderer.sprite.rect.size;
        float pixelToUnits = m_spriteRenderer.sprite.pixelsPerUnit;
        return spriteSize / pixelToUnits;
    }

    //Setting opacity value of sprite renderer
    private void SetOpacity(float opacity)
    {
        var srColor = m_spriteRenderer.color;
        srColor.a = opacity;
        m_spriteRenderer.color = srColor;
    }

    private void CheckClickActions()
    {
        //If its not placed return
        if(!m_isPlaced)return;
        
        //If a right click is performed on it sets this building for attack
        if (Input.GetMouseButtonDown(1) && CheckObjectUnderMouse())
            ProductManager.Instance.OnBuildingSelectedForAttack(this);
        
        //If a left click is performed on it opens info panel
        if (Input.GetMouseButtonDown(0) && CheckObjectUnderMouse())
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                SetOutline(true);
                m_uiManager.OpenInfoPanel(m_buildingType, this);
            }
    }
    
    //Checks is mouse cursor over this building
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

    //This effect plays when damage taken and it will flash an image and disappear after given flash time
    private IEnumerator DamageFlashRoutine(float flashTime)
    {
        m_damageFlash.gameObject.SetActive(true);
        yield return new WaitForSeconds(flashTime);
        m_damageFlash.gameObject.SetActive(false);
    }
    
    //Reducing the health of the building. if health will be 0 or less will destroy the building
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

    //Sets disabled covered tiles enable back. and adds this building to building pool for pooling system
    private void OnDead()
    {
        m_tileManager.EnableCoveredTiles(transform.position, GetRealWorldSize());
        gameObject.SetActive(false);
        FactoryManager.Instance.AddBuildingToPool(this);
        m_uiManager.CheckCanCloseInfoPanel(this);
    }

    //Reset properties when selected from pool system
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
    
    //Sets outline image activation
    public void SetOutline(bool active)
    {
        m_outline.SetActive(active);
    }
}