using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : PersistentSingleton<MainManager>
{
    private SoundManager _soundManager;
    private AgentManager _agentManager;
    public SoundManager SoundManager => _soundManager;
    public AgentManager AgentManager => _agentManager;

    protected override void Awake()
    {
        base.Awake();
        if (!TryGetComponent(out _soundManager))
            _soundManager = gameObject.AddComponent<SoundManager>();

        if (!TryGetComponent(out _agentManager))
            _agentManager = gameObject.AddComponent<AgentManager>();
    }
}
