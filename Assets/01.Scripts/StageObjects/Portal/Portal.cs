using Core;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public interface IPortalObject
{
    public Transform transform { get; }
    public Transform VisualTrm { get; }
}

[RequireComponent(typeof(Rigidbody))]
public class Portal : MonoBehaviour
{
    [SerializeField] private Portal _linkedPortal;
    [SerializeField] private float _angleOffset;

    private Camera _renderCamera;
    private MeshRenderer _meshRenderer;

    private RenderTexture _targetTexture;
    public RenderTexture TargetTexture => _targetTexture;

    private Material _material;

    private Transform _mainCamTrm;
    private float _camDistance;

    private bool _isOverlapping = false;

    private IPortalObject _currentObj;

    private void Awake()
    {
        _renderCamera = transform.Find("Camera").GetComponent<Camera>();
        _meshRenderer = GetComponent<MeshRenderer>();

        _targetTexture = RenderTexture.GetTemporary(256, 256, 16);
        _renderCamera.targetTexture = _targetTexture;

        _material = new Material(_meshRenderer.material);
        _meshRenderer.material = _material;
    }

    private void Start()
    {
        _material.mainTexture = _linkedPortal.TargetTexture;
        _mainCamTrm = Define.MainCam.transform;

        _renderCamera.transform.rotation = Quaternion.Euler(_mainCamTrm.rotation.eulerAngles.x, _angleOffset, 0f);
    }

    private void Update()
    {
        Vector3 camRot = _mainCamTrm.eulerAngles;
        camRot.y += _angleOffset;
        camRot.z = 0f;
        _renderCamera.transform.eulerAngles = camRot;

        if (_isOverlapping)
        {
            Vector3 portalToObj = _currentObj.transform.position - transform.position;

            float rotationDiff = -Quaternion.Angle(transform.rotation, _linkedPortal.transform.rotation);
            rotationDiff += 180f;
            _currentObj.VisualTrm.Rotate(Vector3.up, rotationDiff);

            Vector3 positionOffset = Quaternion.Euler(0f, rotationDiff, 0f) * portalToObj;
            _currentObj.transform.position = _linkedPortal.transform.position + positionOffset;
            
            _isOverlapping = false;

#if UNITY_EDITOR
            EditorApplication.isPaused = true;
#endif
        }

        //UpdateClone();
    }

    private void UpdateClone()
    {
        //foreach(var pair in _clones)
        //{
        //    Transform clone = pair.Value;
        //    Transform origin = pair.Key.transform;
        //    Transform originVisual = pair.Key.VisualTrm;

        //    clone.position = origin.position + new Vector3(3f, 0f, 3f);
        //    clone.rotation = originVisual.rotation;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform clone = null;

        IPortalObject portalComponent = other.GetComponent<IPortalObject>();
        if (portalComponent == null) return;

        //clone = Instantiate(other).transform;
        _isOverlapping = true;
        _currentObj = portalComponent;
        return;

        if(clone == null)
        {
            Debug.LogError($"Failed Create Portal Clone. Object Name: {other.gameObject.name}");
            return;
        }

        Collider[] colliders = clone.GetComponents<Collider>();
        for (int i = 0; i < colliders.Length; i++)
        {
            Destroy(colliders[i]);
        }

        if (clone != null)
        {
            //_clones.Add(portalComponent, clone);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        IPortalObject portalComponent = other.GetComponent<IPortalObject>();
        if (portalComponent == null) return;
        _isOverlapping = false;
        _currentObj = null;
        return;

        ////if (_clones.TryGetValue(portalComponent, out Transform clone))
        //{
        //    //_clones.Remove(portalComponent);
        //    //Destroy(clone.gameObject);
        //}
        //else
        //{
        //    Debug.LogError($"{other.gameObject.name} Doesn't have Portal Clone");
        //}
    }
}
