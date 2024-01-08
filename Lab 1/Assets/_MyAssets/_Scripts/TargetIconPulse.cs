using System.Collections;
using UnityEngine;

public class TargetIconPulse : MonoBehaviour
{
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float m_maxScale = 2f;
    [SerializeField] private float m_minScale = 1f;

    private float m_currentScale = 1f;

    private Coroutine m_pulseCoroutine;

    private void Awake()
    {
        // start the coroutine, if it doesn't yet exist.
        m_pulseCoroutine ??= StartCoroutine(PulseCoroutine());
    }

    private void OnDisable()
    {
        // stop the coroutine, if it exists.
        if (m_pulseCoroutine != null)
        {
            StopCoroutine(m_pulseCoroutine);
            m_pulseCoroutine = null;
        }
    }

    private IEnumerator PulseCoroutine()
    {
        // always run.
        while (true)
        {
            // increase scale loop.
            while (m_currentScale < m_maxScale)
            {
                m_currentScale = Mathf.MoveTowards(m_currentScale, m_maxScale, pulseSpeed * Time.deltaTime);

                transform.localScale = Vector3.one * m_currentScale;

                yield return new WaitForEndOfFrame();
            }
            // decrease scale loop.
            while (m_currentScale > m_minScale)
            {
                m_currentScale = Mathf.MoveTowards(m_currentScale, m_minScale, pulseSpeed * Time.deltaTime);

                transform.localScale = Vector3.one * m_currentScale;
                yield return new WaitForEndOfFrame();
            }

            // handles edge cases where scale is directly equal to max/min scale.
            if (m_currentScale == m_maxScale)
                m_currentScale -= 0.05f;
            else if (m_currentScale == m_minScale)
                m_currentScale += 0.05f;
        }
    }
}
