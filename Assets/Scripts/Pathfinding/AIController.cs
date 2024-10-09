using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{
    public float moveSpeed = 5f;
    private List<Tile> path;
    private int currentPathIndex;
    private bool m_isMoving;
    public bool IsMoving => m_isMoving;

    // Sets the given path and will start movement
    public void SetPath(List<Tile> newPath, Action onComplete)
    {
        path = newPath;
        currentPathIndex = 0;
        StartCoroutine(MoveAlongPath(onComplete));
    }

    //Works until follow all nodes and then throws oncomplete action
    private IEnumerator MoveAlongPath(Action onComplete)
    {
        m_isMoving = true;
        while (currentPathIndex < path.Count)
        {
            Vector3 targetPosition = path[currentPathIndex].transform.position;

            while (Vector3.Distance(transform.position, targetPosition) > 0.001f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }

            currentPathIndex++;
        }
        onComplete?.Invoke();
        m_isMoving = false;
    }
}