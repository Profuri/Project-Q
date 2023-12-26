using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Rigidbody))]
public class PortalObject : MonoBehaviour
{
    [SerializeField] private PortalObject _linkedPortal;
    [SerializeField] private float _angleOffset;

    private Camera _renderCamera;
    private MeshRenderer _meshRenderer;

    private RenderTexture _targetTexture;
    public RenderTexture TargetTexture => _targetTexture;

    private Material _material;

    private Transform _mainCamTrm;
    private float _camDistance;

    private Dictionary<Transform, Transform> _clones = new Dictionary<Transform, Transform>();

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
        Vector3 camForward = _renderCamera.ForwardView().normalized;
        Vector3 dirToCam = (_mainCamTrm.position - _renderCamera.transform.position).normalized;
        camForward.y = dirToCam.y = 0f;

        float angle = Vector3.Angle(camForward, dirToCam);
        if (angle > 270f && angle < 360f) angle -= 360f;

        SetRotation(angle);

        foreach(var pair in _clones)
        {
            pair.Value.position = _linkedPortal.transform.position + pair.Key.parent.localPosition;
            pair.Value.rotation = pair.Key.rotation;
        }
    }

    public void SetRotation(float angle)
    {
        Vector3 prevAngle = _renderCamera.transform.eulerAngles;
        prevAngle.y = angle + _angleOffset;
        _renderCamera.transform.eulerAngles = prevAngle;
    }

    private void OnTriggerEnter(Collider other)
    {

        Transform modelTrm = other.transform.Find("Model");
        if(modelTrm != null)
        {
            other.transform.parent = transform;
            Vector3 spawnPoint = _linkedPortal.transform.position + other.transform.localPosition;

            GameObject obj = Instantiate(modelTrm.gameObject, spawnPoint, modelTrm.rotation);
            obj.transform.parent = transform.root;

            _clones.Add(modelTrm, obj.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Transform modelTrm = other.transform.Find("Model");
        if (modelTrm != null && _clones.TryGetValue(modelTrm, out Transform obj))
        {
            Destroy(obj.gameObject);
            _clones.Remove(modelTrm);

            modelTrm.parent.parent = transform.root;
        }
    }
}
