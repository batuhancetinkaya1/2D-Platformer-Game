using UnityEngine;

public class SensorPlayer : MonoBehaviour
{
    private int m_ColCount = 0;
    private float m_DisableTimer;

    public Collider2D LastCollider { get; private set; }  // Add this line to expose the last collider

    private void OnEnable()
    {
        m_ColCount = 0;
        LastCollider = null;
    }

    public bool State()
    {
        if (m_DisableTimer > 0)
            return false;
        return m_ColCount > 0;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        m_ColCount++;
        LastCollider = other;  // Track the collider that entered
    }

    void OnTriggerExit2D(Collider2D other)
    {
        m_ColCount--;
        if (m_ColCount <= 0)
            LastCollider = null;  // Reset when no colliders are inside
    }

    void Update()
    {
        m_DisableTimer -= Time.deltaTime;
    }

    public void Disable(float duration)
    {
        m_DisableTimer = duration;
    }
}
