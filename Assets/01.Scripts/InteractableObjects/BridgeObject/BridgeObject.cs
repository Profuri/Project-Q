using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BridgeObject : MonoBehaviour
{
    public void Move(Vector3 endValue, float duration)
    {
        transform.DOMove(endValue, duration).SetEase(Ease.OutFlash);
    }
}
