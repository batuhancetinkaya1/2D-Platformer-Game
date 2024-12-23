using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPath : MonoBehaviour
{
    public Vector2 startPosition, endPosition;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(startPosition, endPosition);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(startPosition, 0.1f);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(endPosition, 0.1f);
    }
}