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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Grid"))
        {
            actionHandler.isStuck = true;
        }

        //if (collision.collider.CompareTag("Collectable"))
        //{
        //    //event ile topland���na dair eventi �al��t�r�p b�ylece can ise playera can verece�iz alt�n ise ona g�re �al��t�raca��z 
        //}
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Grid"))
        {
            actionHandler.isStuck = false;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }
}
