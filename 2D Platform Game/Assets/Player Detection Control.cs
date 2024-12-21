using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDetectionControl : MonoBehaviour
{
    private PlayerActionHandler actionHandler;
    private PlayerType playerType;


    private void Awake()
    {
        actionHandler = GetComponent<PlayerActionHandler>();

        playerType = actionHandler.playerType;
    }

    //private void OnCollisionEnter2D(Collision collision)
    //{
    //    if (collision.collider.CompareTag("Grid"))
    //    {
    //        actionHandler.isStuck = true;
    //    }
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Grid"))
        {
            actionHandler.isStuck = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Grid"))
        {
            actionHandler.isStuck = false;
        }
    }
}
