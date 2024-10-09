using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private List<Tile> m_path;
    private int m_currentPathIndex;
    private bool m_isMoving;
    public bool IsMoving => m_isMoving;

    // Sets the given path and will start movement
    public void SetPath(List<Tile> newPath, Action onComplete)
    {
        m_path = newPath;
        m_currentPathIndex = 0;
        StartCoroutine(MoveAlongPath(onComplete));
    }

    //Works until follow all nodes and then throws oncomplete action
    private IEnumerator MoveAlongPath(Action onComplete)
    {
        m_isMoving = true;
        if (m_path != null && m_path.Count > 0)
        {
            while (m_currentPathIndex < m_path.Count)
            {
                Vector3 targetPosition = m_path[m_currentPathIndex].transform.position;

                while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
                {
                    transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                    yield return null;
                }

                m_currentPathIndex++;
            }
        }
        onComplete?.Invoke();
        m_isMoving = false;
    }
}