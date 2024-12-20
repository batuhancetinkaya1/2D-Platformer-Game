using UnityEditor.U2D.Aseprite;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerActionHandler actionHandler;
    private PlayerType playerType;


    [Header("Key Code Keyboard")]
    [SerializeField] public KeyCode rightMoveButton, leftMoveButton, jumpButton, blockButton, attackButton, rollButton;

    [Header("Key Code Joystick")]
    [SerializeField] public KeyCode JoyStick_rightMoveButton, JoyStick_leftMoveButton, JoyStick_jumpButton, JoyStick_blockButton, JoyStick_attackButton, JoyStick_rollButton;

    private void Awake()
    {
        actionHandler = GetComponent<PlayerActionHandler>();
        playerType = actionHandler.playerType;
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        // Horizontal Movement Input (Keyboard or Joystick)
        float horizontalInput = 0f;

        // Horizontal Movement: Keyboard or Joystick
        if (Input.GetKey(rightMoveButton) || Input.GetKey(JoyStick_rightMoveButton))  // Right move
            horizontalInput = 1f;
        else if (Input.GetKey(leftMoveButton) || Input.GetKey(JoyStick_leftMoveButton))  // Left move
            horizontalInput = -1f;
        else if (!Input.GetKey(rightMoveButton) && !Input.GetKey(leftMoveButton) &&
            !Input.GetKey(JoyStick_rightMoveButton) && !Input.GetKey(JoyStick_leftMoveButton))
        {
            horizontalInput = 0f;
        }

        actionHandler.HandleHorizontalMovement(horizontalInput);

        // Jump Input (Keyboard or Joystick)
        if (Input.GetKeyDown(jumpButton) || Input.GetKeyDown(JoyStick_jumpButton))
        {
            Debug.Log("!");
            actionHandler.HandleJump();
        }
           

        // Roll Input (Keyboard or Joystick)
        if (Input.GetKeyDown(rollButton) || Input.GetKeyDown(JoyStick_rollButton))
            actionHandler.HandleRoll();

        // Attack Input (Keyboard or Joystick)
        if (Input.GetKeyDown(attackButton) || Input.GetKeyDown(JoyStick_attackButton))  // Mouse click or joystick button
            actionHandler.HandleAttack();

        // Block Input (Keyboard or Joystick)
        if (Input.GetKeyDown(blockButton) || Input.GetKeyDown(JoyStick_blockButton))  // Mouse right click or joystick button
            actionHandler.HandleBlock(true);
        else if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(JoyStick_blockButton))  // Mouse right click or joystick button
            actionHandler.HandleBlock(false);
    }

}
