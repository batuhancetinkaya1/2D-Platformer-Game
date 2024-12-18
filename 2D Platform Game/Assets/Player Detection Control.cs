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
}
