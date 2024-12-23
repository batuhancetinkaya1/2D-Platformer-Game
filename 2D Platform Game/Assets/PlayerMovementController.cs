using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float m_speed = 4.0f;
    [SerializeField] private float m_jumpForce = 7.5f;
    [SerializeField] private float m_rollForce = 8.0f;
    [SerializeField] private float m_wallSlideSpeed = 2f;
    [SerializeField] private float m_wallStickTime = 1f;
    [SerializeField] private float m_wallJumpTime = 0.2f;
    [SerializeField] private GameObject dustPrefab;


    private PlayerCore m_playerCore;
    private Rigidbody2D m_body2d;
    private SpriteRenderer m_spriteRenderer;

    // Sensörler
    private SensorPlayer m_groundSensor;
    private SensorPlayer m_wallSensorR1, m_wallSensorR2;
    private SensorPlayer m_wallSensorL1, m_wallSensorL2;

    private bool m_grounded = false;
    private bool m_isWallSliding = false;
    private bool m_wallJumping = false;
    private bool m_isStuck = false;
    private float m_wallJumpingCurrentTime = 0f;
    private float m_wallStickCounter = 0f;
    private int m_wallSlidingSide = 0;
    private int m_facingDirection = 1;

    // Roll
    private bool m_rolling = false;
    private float m_rollDuration = 8.0f / 14.0f;
    private float m_rollCurrentTime;

    public bool IsRolling => m_rolling;
    public bool IsGrounded => m_grounded;
    public bool IsWallSliding => m_isWallSliding;
    public int FacingDirection => m_facingDirection;

    public bool IsStuck
    {
        get => m_isStuck;
        set
        {
            if (m_isStuck != value)
            {
                m_isStuck = value;
            }
        }
    }

    public void Initialize(PlayerCore core)
    {
        m_playerCore = core;
    }

    private void Awake()
    {
        m_body2d = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        // Sensörleri bul
        m_groundSensor = transform.Find("GroundSensor").GetComponent<SensorPlayer>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<SensorPlayer>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<SensorPlayer>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<SensorPlayer>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<SensorPlayer>();
    }

    private void Update()
    {
        CheckRollState();
        CheckWallJumpState();
        CheckGrounded();
        HandleWallSliding();

        if (m_playerCore && m_playerCore.AnimControl)
        {
            m_playerCore.AnimControl.SetAirSpeedY(m_body2d.velocity.y);
        }
    }

    #region Horizontal Movement
    public void HandleHorizontalMovement(float inputX)
    {
        bool isStuck = m_isStuck;
        // Duvar sensörleri
        if ((m_wallSensorL1.State() && !m_wallSensorL2.State()) ||
            (m_wallSensorR1.State() && !m_wallSensorR2.State()))
        {
            return;
        }

        if (!m_wallSensorL1.State() && !m_wallSensorL2.State() &&
            !m_wallSensorR1.State() && !m_wallSensorR2.State() &&
            !m_grounded && !m_isWallSliding && isStuck)
        {
            return;
        }

        if ((!m_wallSensorL1.State() && m_wallSensorL2.State() && !m_grounded && !m_isWallSliding && isStuck) ||
            (!m_wallSensorR1.State() && m_wallSensorR2.State() && !m_grounded && !m_isWallSliding && isStuck))
        {
            return;
        }

        // Duvarda kayma
        if (m_isWallSliding)
        {
            // Zýt yöne hamle
            if ((m_wallSlidingSide > 0 && inputX < 0) ||
                (m_wallSlidingSide < 0 && inputX > 0))
            {
                m_isWallSliding = false;
                m_playerCore.AnimControl.SetWallSliding(false);
                m_playerCore.AnimControl.SetTriggerJump();
            }

            // Yavaþ yatay kayma
            m_body2d.velocity = new Vector2(inputX * (m_speed * 0.5f), m_body2d.velocity.y);
            return;
        }

        // Yön belirleme
        if (inputX > 0)
        {
            m_spriteRenderer.flipX = false;
            m_playerCore.DetectionControl.UpdateMeleeSensorScale(1);
            m_facingDirection = 1;
        }
        else if (inputX < 0)
        {
            m_spriteRenderer.flipX = true;
            m_playerCore.DetectionControl.UpdateMeleeSensorScale(-1);
            m_facingDirection = -1;
        }

        // Roll veya wallJump yapýlmýyorsa normal hareket
        if (!m_rolling && !m_wallJumping)
        {
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        }

        // Animasyon
        if (m_playerCore && m_playerCore.AnimControl)
            m_playerCore.AnimControl.SetRunningState(inputX);
    }
    #endregion

    #region Jump
    public void HandleJump()
    {
        if (m_grounded && !m_rolling)
        {
            PerformJump(m_jumpForce);
        }
        else if (m_isWallSliding)
        {
            PerformWallJump();
        }
    }

    private void PerformJump(float jumpForce)
    {
        m_playerCore.AnimControl.SetTriggerJump();
        m_grounded = false;
        m_playerCore.AnimControl.SetGrounded(m_grounded);

        m_body2d.velocity = new Vector2(m_body2d.velocity.x, jumpForce);
        m_groundSensor.Disable(0.2f);
    }

    private void PerformWallJump()
    {
        m_playerCore.AnimControl.SetTriggerJump();
        m_isWallSliding = false;
        m_playerCore.AnimControl.SetWallSliding(false);

        m_body2d.velocity = new Vector2(-m_wallSlidingSide * m_jumpForce * 0.75f, m_jumpForce * 0.75f);
        m_wallJumping = true;
        m_wallJumpingCurrentTime = 0f;

        if (m_wallSlidingSide > 0)
        {
            m_wallSensorR1.Disable(0.3f);
            m_wallSensorR2.Disable(0.3f);
        }
        else
        {
            m_wallSensorL1.Disable(0.3f);
            m_wallSensorL2.Disable(0.3f);
        }

        m_spriteRenderer.flipX = (-m_wallSlidingSide < 0);
        m_facingDirection = (-m_wallSlidingSide < 0) ? -1 : 1;
    }
    #endregion

    #region Roll
    public void HandleRoll()
    {
        if (!m_rolling)
        {
            m_rolling = true;
            m_rollCurrentTime = 0f;
            m_playerCore.AnimControl.SetTriggerRoll();

            if (m_isWallSliding)
            {
                m_isWallSliding = false;
                m_playerCore.AnimControl.SetWallSliding(false);
                m_body2d.velocity = new Vector2(-m_wallSlidingSide * m_rollForce, m_body2d.velocity.y);

                if (m_wallSlidingSide > 0)
                {
                    m_wallSensorR1.Disable(0.3f);
                    m_wallSensorR2.Disable(0.3f);
                }
                else
                {
                    m_wallSensorL1.Disable(0.3f);
                    m_wallSensorL2.Disable(0.3f);
                }

                m_spriteRenderer.flipX = (-m_wallSlidingSide < 0);
                m_facingDirection = (-m_wallSlidingSide < 0) ? -1 : 1;
            }
            else
            {
                m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
            }

            // Collider boyutu kýsaltma (isteðe baðlý)
            BoxCollider2D colliderToChange = m_body2d.GetComponent<BoxCollider2D>();
            colliderToChange.offset = new Vector2(0, 0.2f);
            colliderToChange.size = new Vector2(colliderToChange.size.x, 0.3f);
        }
    }
    #endregion

    #region Private Helpers
    private void CheckRollState()
    {
        if (m_rolling)
        {
            m_rollCurrentTime += Time.deltaTime;
            if (m_rollCurrentTime > m_rollDuration)
            {
                // Normal collider boyutuna dön
                BoxCollider2D colliderToChange = m_body2d.GetComponent<BoxCollider2D>();
                colliderToChange.offset = new Vector2(0, 0.670486f);
                colliderToChange.size = new Vector2(colliderToChange.size.x, 1.183028f);
                m_rolling = false;
            }
        }
    }

    private void CheckWallJumpState()
    {
        if (m_wallJumping)
        {
            m_wallJumpingCurrentTime += Time.deltaTime;
            if (m_wallJumpingCurrentTime >= m_wallJumpTime)
            {
                m_wallJumping = false;
            }
        }
    }

    private void CheckGrounded()
    {
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_playerCore.AnimControl.SetGrounded(m_grounded);
        }
        else if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_playerCore.AnimControl.SetGrounded(m_grounded);
        }
    }

    private void HandleWallSliding()
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

            m_spriteRenderer.flipX = (m_wallSlidingSide < 0);
        }
        else
        {
            if (m_isWallSliding)
            {
                m_isWallSliding = false;
                m_playerCore.AnimControl.SetWallSliding(false);
            }
            m_wallStickCounter = m_wallStickTime;
        }

        m_playerCore.AnimControl.SetWallSliding(m_isWallSliding);
    }

    public void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        if (dustPrefab != null)
        {
            GameObject dust = Instantiate(dustPrefab, spawnPosition, transform.localRotation);
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }
    #endregion
}
