using DG.Tweening;
using TinyGiantStudio.Text;
using UnityEngine;

public class UIYesNoButton3D : UIButton3D
{
    [SerializeField] private Material _yesMaterial;
    [SerializeField] private Material _noMaterial;

    private Renderer _renderer;

    protected override void Awake()
    {
        base.Awake();
        _renderer = GetComponent<Renderer>();
    }

    public void SettingActive(bool active)
    {
        _3dText.Material = active ? _noMaterial : _yesMaterial;
        _renderer.material = active ? _yesMaterial : _noMaterial;
        transform.DOLocalMoveY(active ? 0.125f : -0.09f, 0.2f);
    }
    
    public override void OnHoverHandle()
    {
        transform.DOScaleY(_originScale.y * 1.75f, 0.2f).SetUpdate(true);
    }

    public override void OnHoverCancelHandle()
    {
        transform.DOScaleY(_originScale.y, 0.2f).SetUpdate(true);
    }
}