using UnityEngine;

public class AudioSliderIcon : MonoBehaviour
{
    private GameObject _muteIcon;
    private GameObject _smallIcon;
    private GameObject _middleIcon;
    private GameObject _maxIcon;

    private void Awake()
    {
        _muteIcon = transform.Find("Mute").gameObject;
        _smallIcon = transform.Find("Small").gameObject;
        _middleIcon = transform.Find("Middle").gameObject;
        _maxIcon = transform.Find("Max").gameObject;
    }

    public void ChangeVolumeHandle(float percent)
    {
        _maxIcon.SetActive(percent >= 0.7);
        _middleIcon.SetActive(percent >= 0.4f);
        _smallIcon.SetActive(percent > 0f);
        _muteIcon.SetActive(percent == 0);
    }
}
