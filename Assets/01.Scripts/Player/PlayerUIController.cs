using UnityEngine;
using static Core.Define;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private RectTransform _keyGuideUI;

    private void Update()
    {
        _keyGuideUI.rotation = Quaternion.LookRotation(MainCam.transform.forward);
    }

    public void SetKeyGuide(bool value)
    {
        _keyGuideUI.gameObject.SetActive(value);
    }
}
