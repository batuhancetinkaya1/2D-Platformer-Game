using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionControl : MonoBehaviour
{
    private PlayerActionHandler actionHandler;
    private PlayerType playerType;

    private float m_enemyDamage = 5f;
    private void Awake()
    {
        actionHandler = GetComponent<PlayerActionHandler>();
        playerType = actionHandler.playerType;
    }

    private void OnCollision2D(Collision collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            actionHandler.GetDamage(m_enemyDamage);
        }
    }
}
