using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    private Transform m_playerTransform;
    private Camera m_mainCamera;

    private Vector3 m_offset;
    private float m_smoothSpeed = 0.125f;
    private float m_defaultZoom;
    private bool m_isRespawning;

    [Header("CatEncounter Settings")]
    [SerializeField] private float m_catEncounterZoom = 5f;
    [SerializeField] private float m_catEncounterSmoothTime = 0.5f;

    [Header("FinalFight Settings")]
    [SerializeField] private Transform m_finalFightCenter;
    [SerializeField] private float m_finalFightZoom = 10f;

    [Header("Respawn Settings")]
    [SerializeField] private float m_respawnWaitTime = 1.5f;

    private void Awake()
    {
        m_mainCamera = Camera.main;
        m_playerTransform = GameObject.FindGameObjectWithTag("Player").transform;

        // Varsayýlan deðerler
        m_defaultZoom = m_mainCamera.orthographicSize;
        m_offset = new Vector3(0, 0, -10);

        // Eðer FinalFightCenter sahnede etiketli bir objeyse:
        // var finalFightObject = GameObject.FindGameObjectWithTag("FinalFightCenter");
        // if (finalFightObject != null)
        //     m_finalFightCenter = finalFightObject.transform;
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChange += HandleGameStateChange;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChange -= HandleGameStateChange;
    }

    private void LateUpdate()
    {
        // GameOn durumunda kamera, player'ý takip ediyor
        if (GameManager.Instance.CurrentState == GameStates.GameOn && !m_isRespawning)
        {
            Vector3 targetPosition = m_playerTransform.position + m_offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, m_smoothSpeed);
        }
    }

    private void HandleGameStateChange(GameStates newState)
    {
        switch (newState)
        {
            case GameStates.GameOn:
                ResetCamera();
                break;

            case GameStates.CatEncounter:
                FocusOnPlayer(m_catEncounterZoom, m_catEncounterSmoothTime);
                break;

            case GameStates.FinalFight:
                FocusOnFinalFight();
                break;

            case GameStates.Respawn:
                StartRespawnSequence();
                break;

            case GameStates.GameOver:
                HandleGameOver();
                break;
        }
    }

    private void ResetCamera()
    {
        m_mainCamera.orthographicSize = m_defaultZoom;
        m_offset = new Vector3(0, 0, -10);
    }

    private void FocusOnPlayer(float zoom, float smoothTime)
    {
        StartCoroutine(SmoothZoom(zoom, smoothTime));
        transform.position = new Vector3(
            m_playerTransform.position.x,
            m_playerTransform.position.y,
            transform.position.z
        );
    }

    private void FocusOnFinalFight()
    {
        if (m_finalFightCenter != null)
        {
            m_mainCamera.orthographicSize = m_finalFightZoom;
            transform.position = new Vector3(
                m_finalFightCenter.position.x,
                m_finalFightCenter.position.y,
                transform.position.z
            );
        }
    }

    private void StartRespawnSequence()
    {
        m_isRespawning = true;
        StartCoroutine(RespawnCameraFreeze());
    }

    private IEnumerator RespawnCameraFreeze()
    {
        yield return new WaitForSeconds(m_respawnWaitTime);
        m_isRespawning = false;
        ResetCamera();
    }

    private void HandleGameOver()
    {
        Debug.Log("Game Over - Camera Stopped Following Player");
    }

    private IEnumerator SmoothZoom(float targetZoom, float duration)
    {
        float startZoom = m_mainCamera.orthographicSize;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            m_mainCamera.orthographicSize = Mathf.Lerp(startZoom, targetZoom, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        m_mainCamera.orthographicSize = targetZoom;
    }
}
