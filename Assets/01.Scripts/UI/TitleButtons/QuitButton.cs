using AxisConvertSystem;
using InteractableSystem;
using UnityEngine;

public class QuitButton : InteractableObject
{
    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        // 나중에 연출 추가하던지 하자
        Application.Quit();
    }
}