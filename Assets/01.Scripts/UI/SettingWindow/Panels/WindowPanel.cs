using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WindowPanel : MonoBehaviour
{
    protected SettingWindow _settingWindow;
    
    public virtual void Init(SettingWindow settingWindow)
    {
        gameObject.SetActive(false);
        _settingWindow = settingWindow;
    }
    

    public virtual void LoadPanel()
    {
        gameObject.SetActive(true);
    }

    public virtual void ReleasePanel()
    {
        gameObject.SetActive(false);
    }
}