using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] TextMeshProUGUI m_stateText;
    [SerializeField] TextMeshProUGUI m_possibleTransitionsText;
    [SerializeField] TextMeshProUGUI m_outcomeText;

    [SerializeField] private GameObject m_intro, m_play, m_end;
    protected void Start()
    {
        GuardStateMachine.OnStateChanged += UpdateStateUI;
        GameManager.OnGameWin += DisplayWinText;
        GameManager.OnGameLose += DisplayLoseText;
    }

    public void SetUI(int currentScene)
    {
            m_intro.SetActive(currentScene == 0);
            m_play.SetActive(currentScene == 1);
            m_end.SetActive(currentScene == 2);        
    }

    private void DisplayLoseText()
    {
        m_outcomeText.text = "You lose! Play Again?";
    }

    private void DisplayWinText()
    {
        m_outcomeText.text = "You win! Play Again?";
    }

    private void UpdateStateUI(object sender, GuardStateMachine.State state)
    {
        GuardStateMachine gsm = (GuardStateMachine)sender;
        m_stateText.text = state.ToString();
        m_possibleTransitionsText.text = "";
        var transitions = gsm.StateTransitionDictionary[gsm.GetCurrentState()];
        foreach (var transition in transitions )
        {
            m_possibleTransitionsText.text += transition.Next.ToString() + '\n';
        }
    }
}
