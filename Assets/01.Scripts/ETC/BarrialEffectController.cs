using DG.Tweening;
using UnityEngine;

public class BarrialEffectController : MonoBehaviour
{
    private Collider _collider;
    private Material _material;

    private float _originThreshold;

    private readonly int _thresholdHash = Shader.PropertyToID("_Threshold");
    private readonly int _opacityHash = Shader.PropertyToID("_Opacity");

    [SerializeField] private float _destroyPracTime;
    [SerializeField] private float _destroyTime;
    
    [SerializeField] private float _appearTime;

    private SoundEffectPlayer _soundEffectPlayer;

    private void Awake()
    {
        Debug.Log(1);
        _collider = GetComponent<Collider>();
        _collider.enabled = false;

        _material = GetComponent<Renderer>().material;
        _originThreshold = _material.GetFloat(_thresholdHash);
        _material.SetFloat(_thresholdHash, 1f);
        _material.SetFloat(_opacityHash, 0f);
        _soundEffectPlayer = new SoundEffectPlayer(this);
    }

    public void Appear()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        var seq = DOTween.Sequence();
        seq.Join(DOTween.To(
            () => _material.GetFloat(_thresholdHash),
            threshold => _material.SetFloat(_thresholdHash, threshold),
            _originThreshold, _appearTime
        ));
        seq.Join(DOTween.To(
            () => _material.GetFloat(_opacityHash),
            opacity => _material.SetFloat(_opacityHash, opacity),
            1f, _appearTime
        ));
        seq.OnComplete(() => _collider.enabled = true);
        SoundManager.Instance.PlaySFX("Barrier", true, _soundEffectPlayer);
    }

    public void Destroy()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        Debug.Log("Destroy");
        
        var seq = DOTween.Sequence();
        seq.Join(DOTween.To(
            () => _material.GetFloat(_thresholdHash),
            threshold => _material.SetFloat(_thresholdHash, threshold),
            0, _destroyPracTime
        ));
        seq.Append(DOTween.To(
            () => _material.GetFloat(_thresholdHash),
            threshold => _material.SetFloat(_thresholdHash, threshold),
            1f, _destroyTime
        ));
        seq.Join(DOTween.To(
            () => _material.GetFloat(_opacityHash),
            opacity => _material.SetFloat(_opacityHash, opacity),
            0f, _destroyTime
        ));
        seq.OnComplete(() => _collider.enabled = false);
        _soundEffectPlayer.Stop();
    }
}
