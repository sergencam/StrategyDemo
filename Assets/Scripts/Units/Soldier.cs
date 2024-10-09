using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Abstract class representing a soldier in the game. This class handles basic soldier actions like selection,
/// movement, attack range checking, and deselection.
/// </summary>
public abstract class Soldier : MonoBehaviour
{
    [SerializeField] protected float m_damage;                   // Damage dealt by the soldier
    [SerializeField] protected float m_health = 10f;             // Health of the soldier
    [SerializeField] protected GameObject m_outline;             // Outline used to indicate selection
    [SerializeField] protected float m_attackRange;                         // Public property to access damage value
    private bool m_isSelected;                                   // Boolean flag to check if soldier is selected
    private AIController m_aiController;                         // Reference to the AI controller component
    private TileManager m_tileManager;
    private ProductManager m_productManager;
    private UIManager m_uiManager;

    /// <summary>
    /// Called when the script instance is being loaded. Initializes the AIController.
    /// </summary>
    private void Awake()
    {
        m_aiController = GetComponent<AIController>();
    }

    private void Start()
    {
        m_tileManager = TileManager.Instance;
        m_productManager = ProductManager.Instance;
        m_uiManager = UIManager.Instance;
    }

    /// <summary>
    /// Called once per frame to handle soldier updates like movement.
    /// </summary>
    private void Update()
    {
        CheckForMoveToTarget();
    }

    /// <summary>
    /// Handles movement of the soldier to a target point based on input and pathfinding.
    /// </summary>
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

    /// <summary>
    /// Handles selection logic when the soldier is clicked on.
    /// </summary>
    private void OnMouseDown()
    {
        if (Input.GetMouseButtonDown(0) && m_isSelected == false && m_aiController.IsMoving == false)
            StartCoroutine(OnSelectAsync());
    }

    /// <summary>
    /// Coroutine to handle soldier selection and outline activation.
    /// </summary>
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

    /// <summary>
    /// Deselects the soldier, disabling its outline and selection status.
    /// </summary>
    private void OnDeselect()
    {
        m_isSelected = false;
        m_outline.SetActive(false);
        m_productManager.LastSelectedSoldier = null;
    }

    /// <summary>
    /// Checks if the target position is within the attack range of the soldier.
    /// </summary>
    /// <param name="targetPos">The position of the target.</param>
    /// <returns>Returns true if the target is within attack range, false otherwise.</returns>
    private bool CheckIsAttackRangeEnough(Vector3 targetPos)
    {
        Vector3 currentPos = transform.position;
        float dist = Vector3.Distance(currentPos, targetPos);
        print(dist);
        return dist < m_attackRange;
    }

    public void OnAttackToBuilding(Building building)
    {
        var buildingClosestPoint = building.GetComponent<Collider2D>().ClosestPoint(new(transform.position.x, transform.position.y));
        if (CheckIsAttackRangeEnough(buildingClosestPoint))
            building.OnTakeDamage(m_damage);
    }
}