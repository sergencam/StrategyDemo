using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Soldier : MonoBehaviour
{
    [SerializeField] protected float m_damage;
    [SerializeField] protected float m_health = 10f;
    [SerializeField] protected GameObject m_outline;
    [SerializeField] protected float m_attackRange;
    private bool m_isSelected;
    private AIController m_aiController;
    private TileManager m_tileManager;
    private ProductManager m_productManager;

    private void Awake()
    {
        m_aiController = GetComponent<AIController>();
    }

    private void Start()
    {
        m_tileManager = TileManager.Instance;
        m_productManager = ProductManager.Instance;
    }

    private void Update()
    {
        CheckForMoveToTarget();
    }

    private void CheckForMoveToTarget()
    {
        if (m_isSelected && m_aiController.IsMoving == false && Input.GetMouseButtonDown(0))
        {
            int soldierLayerMask = LayerMask.GetMask("Soldier");
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero, Mathf.Infinity, soldierLayerMask);
            bool isClickedToSoldier = hit.collider != null;

            if (!EventSystem.current.IsPointerOverGameObject() && !isClickedToSoldier)
                MoveToTarget();
            OnDeselect();
        }
    }

    private void MoveToTarget()
    {
        m_aiController.SetPath(m_tileManager.FindPath(transform.position), OnPathComplete);
    }

    private void OnPathComplete()
    {
        if (m_productManager.LastSelectedSoldier == null)
            OnSelect();
        else
            OnDeselect();
    }

    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && m_isSelected == false && m_aiController.IsMoving == false)
            StartCoroutine(OnSelectAsync());
    }

    private IEnumerator OnSelectAsync()
    {
        m_outline.SetActive(true);
        yield return new WaitForEndOfFrame();
        m_isSelected = true;
        m_productManager.LastSelectedSoldier = this;
    }
    
    public void OnSelect()
    {
        m_outline.SetActive(true);
        m_isSelected = true;
        if(m_productManager)
            m_productManager.LastSelectedSoldier = this;
    }

    private void OnDeselect()
    {
        m_isSelected = false;
        m_outline.SetActive(false);
        m_productManager.LastSelectedSoldier = null;
    }
    
    private bool CheckIsAttackRangeEnough(Vector3 targetPos)
    {
        Vector3 currentPos = transform.position;
        float dist = Vector3.Distance(currentPos, targetPos);
        return dist < m_attackRange;
    }

    public void OnAttackToBuilding(Building building)
    {
        var buildingClosestPoint = building.GetComponent<Collider2D>().ClosestPoint(new(transform.position.x, transform.position.y));
        if (CheckIsAttackRangeEnough(buildingClosestPoint))
            building.OnTakeDamage(m_damage);
    }
}