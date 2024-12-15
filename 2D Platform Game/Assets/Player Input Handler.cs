using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerMoveHandler moveHandler;
    private PlayerAnimControl animController;

    private void Awake()
    {
        moveHandler = GetComponent<PlayerMoveHandler>();
        animController = GetComponent<PlayerAnimControl>();
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        // Horizontal Movement Input
        float horizontalInput = Input.GetAxis("Horizontal");
        moveHandler.HandleHorizontalMovement(horizontalInput);

        // Jump Input
        if (Input.GetKeyDown("space"))
            moveHandler.HandleJump();

        // Roll Input
        else if (Input.GetKeyDown("left shift"))
            moveHandler.HandleRoll();

        // Attack Input
        else if (Input.GetMouseButtonDown(0))
            moveHandler.HandleAttack();

        // Block Input
        else if (Input.GetMouseButtonDown(1))
            moveHandler.HandleBlock(true);
        else if (Input.GetMouseButtonUp(1))
            moveHandler.HandleBlock(false);


        //// Special Action Inputs
        //if (Input.GetKeyDown("e"))
        //    animController.HandleDeath();

        //if (Input.GetKeyDown("q"))
        //    animController.HandleHurt();
    }
}
