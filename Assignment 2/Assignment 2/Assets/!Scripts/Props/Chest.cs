using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : ActorBase
{
    public void Open()
    {
        _anim.ResetTrigger("_CloseMe");
        _anim.SetTrigger("_OpenMe");
        if (AudioManager.Instance.CurrentlyPlaying == false)
            AudioManager.Instance.PlaySoundEffect("Complete", 0.2f);
    }

    public void Close()
    {
        _anim.SetTrigger("_CloseMe");
    }
}
