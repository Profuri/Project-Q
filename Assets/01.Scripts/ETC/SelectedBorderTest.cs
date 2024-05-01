using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Collider))]
public class SelectedBorderTest : MonoBehaviour
{
    [SerializeField] private SelectedBorder _selectedBorder;
    private Collider _collider;

    private void Awake()
    {
        _collider = GetComponent<Collider>();
        _selectedBorder = Instantiate(_selectedBorder);
        _selectedBorder.Setting(_collider);
    }

    private void Update()
    {
        _selectedBorder?.Setting(_collider);
    }
}
