using UnityEngine;

public class PlayerActionHandler : MonoBehaviour
{
    [SerializeField] float m_speed = 4.0f;
    [SerializeField] float m_jumpForce = 7.5f;
    [SerializeField] float m_rollForce = 6.0f;
    [SerializeField] float m_wallSlideSpeed = 2f;
    [SerializeField] float m_wallStickTime = 1f;
    [SerializeField] float m_wallJumpTime = 0.2f;
    [SerializeField] public PlayerType playerType;
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
    private bool m_wallJumping = false;
    private bool m_isBlocking = false;
    private int m_facingDirection = 1;
    private int m_currentAttack = 0;
    private float m_timeSinceAttack = 0.0f;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;
    private float m_wallJumpingCurrentTime = 0.0f;
    private float m_wallStickCounter = 0f;
    private float m_health;
    private int m_wallSlidingSide = 0;

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

        if (m_wallJumping)
        {
            m_wallJumpingCurrentTime += Time.deltaTime;
            if (m_wallJumpingCurrentTime >= m_wallJumpTime)
            {
                m_wallJumping = false; // Wall jump süresi bitince, m_wallJumping'i false yap
            }
        }

        CheckGroundedStatus();
        HandleWallSlide();

        playerAnimControl.SetAirSpeedY(m_body2d.velocity.y);
    }

    private void CheckGroundedStatus()
    {
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
    }

    private void HandleWallSlide()
    {
        bool onRightWall = m_wallSensorR1.State() && m_wallSensorR2.State();
        bool onLeftWall = m_wallSensorL1.State() && m_wallSensorL2.State();

        if ((onRightWall || onLeftWall) && !m_grounded)
        {
            m_isWallSliding = true;
            m_wallSlidingSide = onRightWall ? 1 : -1;

            if (m_wallStickCounter > 0)
            {
                m_wallStickCounter -= Time.deltaTime;
                m_body2d.velocity = new Vector2(m_body2d.velocity.x, 0);
            }
            else
            {
                m_body2d.velocity = new Vector2(m_body2d.velocity.x, -m_wallSlideSpeed);
            }

            m_spriteRenderer.flipX = m_wallSlidingSide < 0;
        }
        else
        {
            if (m_isWallSliding)
            {
                m_isWallSliding = false;
                playerAnimControl.SetWallSliding(false);
            }

            m_wallStickCounter = m_wallStickTime;
        }

        playerAnimControl.SetWallSliding(m_isWallSliding);
    }

    public void HandleHorizontalMovement(float inputX)
    {
        if (m_isWallSliding)
        {
            // Allow movement in any direction while wall sliding
            // Moving away from the wall will cause the player to leave wall slide
            if ((m_wallSlidingSide > 0 && inputX < 0) ||
                (m_wallSlidingSide < 0 && inputX > 0))
            {
                m_isWallSliding = false;
                playerAnimControl.SetWallSliding(false);
                playerAnimControl.SetTriggerJump();
            }

            // Allow some horizontal movement while wall sliding
            m_body2d.velocity = new Vector2(inputX * (m_speed * 0.5f), m_body2d.velocity.y);
            return;
        }

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

        if (!m_rolling && ! m_wallJumping)
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        playerAnimControl.SetRunningState(inputX);
    }

    public void HandleJump()
    {
        if (m_grounded && !m_rolling)
        {
            PerformJump(m_jumpForce);
        }
        else if (m_isWallSliding)
        {
            PerformWallJump(m_jumpForce);
        }
    }

    private void PerformJump(float jumpForce)
    {
        playerAnimControl.SetTriggerJump();
        m_grounded = false;
        playerAnimControl.SetGrounded(m_grounded);
        m_body2d.velocity = new Vector2(m_body2d.velocity.x, jumpForce);
        m_groundSensor.Disable(0.2f);
    }
    private void PerformWallJump(float jumpForce)
    {
        playerAnimControl.SetTriggerJump();
        m_isWallSliding = false;
        playerAnimControl.SetWallSliding(m_isWallSliding);
        m_body2d.velocity = new Vector2(-m_wallSlidingSide * m_jumpForce * 0.4f, jumpForce * 0.5f );

        // Wall jump sonrasý yatay hareketi kontrol et
        m_wallJumping = true;
        m_wallJumpingCurrentTime = 0.0f;

        if (m_wallSlidingSide > 0)
        {
            m_wallSensorR1.Disable(0.3f);
            m_wallSensorR2.Disable(0.3f);
        }
        else if (m_wallSlidingSide < 0)
        {
            m_wallSensorL1.Disable(0.3f);
            m_wallSensorL2.Disable(0.3f);
        }

        m_spriteRenderer.flipX = -m_wallSlidingSide < 0;
        m_facingDirection = -m_wallSlidingSide < 0 ? -1 : 1;
    }

    public void HandleRoll()
    {
        // Allow rolling while wall sliding
        if (!m_rolling)
        {
            m_rolling = true;
            m_rollCurrentTime = 0f;
            playerAnimControl.SetTriggerRoll();

            // If wall sliding, roll away from the wall
            if (m_isWallSliding)
            {
                m_isWallSliding = false;
                playerAnimControl.SetWallSliding(false);
                m_body2d.velocity = new Vector2(-m_wallSlidingSide * m_rollForce, m_body2d.velocity.y);

                if (m_wallSlidingSide > 0)
                {
                    m_wallSensorR1.Disable(0.3f);
                    m_wallSensorR2.Disable(0.3f);
                }
                else if (m_wallSlidingSide < 0)
                {
                    m_wallSensorL1.Disable(0.3f);
                    m_wallSensorL2.Disable(0.3f);
                }
                m_spriteRenderer.flipX = -m_wallSlidingSide < 0;
                m_facingDirection = -m_wallSlidingSide < 0 ? -1 : 1;
            }
            else
            {
                m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
            }
        }
    }

    public void HandleAttack()
    {
        // Prevent attack while wall sliding
        if (m_isWallSliding) return;

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
        // Prevent blocking while wall sliding
        if (m_isWallSliding) return;

        if (!m_rolling)
        {
            if (isBlocking)
            {
                //m_body2d.velocity = new Vector2(0, 0);
                m_isBlocking = true;
                playerAnimControl.SetTriggerBlock();
                playerAnimControl.SetIdleBlock(true);
            }
            else
            {
                m_isBlocking = false;
                playerAnimControl.SetIdleBlock(false);
            }
        }
    }

    public void GetDamage(float damage)
    {
        if (m_isBlocking)
        {
            m_health -= damage / 5;
            //Settrigger blocked
        }
        else
        {
            m_health -= damage;
            //hurt
        }

        if(m_health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //die
        //m_isAlive
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