using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WindowPanel : MonoBehaviour
{
    public virtual void Init(SettingWindow settingWindow)
    {
        gameObject.SetActive(false);
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