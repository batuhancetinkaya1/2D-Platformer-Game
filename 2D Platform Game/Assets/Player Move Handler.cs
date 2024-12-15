using UnityEngine;

public class PlayerMoveHandler : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] GameObject m_slideDust;

    private PlayerAnimControl playerAnimControl;
    private Rigidbody2D m_body2d;
    private SensorPlayer m_groundSensor;
    private SensorPlayer m_wallSensorR1;
    private SensorPlayer m_wallSensorR2;
    private SensorPlayer m_wallSensorL1;
    private SensorPlayer m_wallSensorL2;
    private SpriteRenderer m_spriteRenderer;

    private bool m_isWallSliding = false;
    private bool m_grounded = false;
    private bool m_rolling = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    private void Awake()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        playerAnimControl = GetComponent<PlayerAnimControl>();

        m_groundSensor = transform.Find("GroundSensor").GetComponent<SensorPlayer>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<SensorPlayer>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<SensorPlayer>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<SensorPlayer>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<SensorPlayer>();
    }

    private void Update()
    {
        m_timeSinceAttack += Time.deltaTime;

        if (m_rolling)
        {
            m_rollCurrentTime += Time.deltaTime;

            if (m_rollCurrentTime > m_rollDuration)
                m_rolling = false;
        }

        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            playerAnimControl.SetGrounded(m_grounded);
        }

        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            playerAnimControl.SetGrounded(m_grounded);
        }

        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        playerAnimControl.SetWallSliding(m_isWallSliding);

        playerAnimControl.SetAirSpeedY(m_body2d.velocity.y);
    }

    public void HandleHorizontalMovement(float inputX)
    {
        if (inputX > 0)
        {
            m_spriteRenderer.flipX = false;
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            m_spriteRenderer.flipX = true;
            m_facingDirection = -1;
        }

        if (!m_rolling)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        playerAnimControl.SetRunningState(inputX);
    }

    public void HandleJump()
    {
        if (m_grounded && !m_rolling)
        {
            playerAnimControl.SetTriggerJump();
            m_grounded = false;
            playerAnimControl.SetGrounded(m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }
    }

    public void HandleRoll()
    {
        if (!m_rolling /*&& !m_isWallSliding*/)
        {
            m_rolling = true;
            m_rollCurrentTime = 0f;
            playerAnimControl.SetTriggerRoll();
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }
    }

    public void HandleAttack()
    {
        if (m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            if (m_currentAttack > 3)
                m_currentAttack = 1;

            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            playerAnimControl.SetTriggerAttack(m_currentAttack);
            m_timeSinceAttack = 0.0f;
        }
    }

    public void HandleBlock(bool isBlocking)
    {
        if (!m_rolling && !m_isWallSliding)
        {
            if (isBlocking)
            {
                playerAnimControl.SetTriggerBlock();
                playerAnimControl.SetIdleBlock(true);
            }
            else
            {
                playerAnimControl.SetIdleBlock(false);
            }
        }
    }

    public void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (m_slideDust != null)
        {
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation);
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
}