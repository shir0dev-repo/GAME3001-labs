using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LoadSceneOnCollision : MonoBehaviour
{
    private const string _PLAYER_TAG = "Player";

    [SerializeField] private Collider2D m_collider;
    [SerializeField] private int m_nextSceneIndex = 2;
    private void Awake()
    {
        if (m_collider == null)
            m_collider = GetComponent<Collider2D>();
        m_collider.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // only care about player, return.
        if (!collision.CompareTag(_PLAYER_TAG)) return;

        SceneLoader.LoadSceneByIndex(m_nextSceneIndex);
    }
}
