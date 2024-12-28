using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    public Vector2 m_startPosition, m_endPosition;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(m_startPosition, m_endPosition);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(m_startPosition, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_endPosition, 0.1f);
    }
}