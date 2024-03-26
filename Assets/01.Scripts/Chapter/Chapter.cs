using InteractableSystem;
using AxisConvertSystem;
using UnityEngine;

public class Chapter : InteractableObject
{
    [SerializeField] private ChapterData _data;
    [SerializeField] private float _symbolRotateSpeed;

    private Transform _symbolTrm;

    public override void Awake()
    {
        base.Awake();
        _symbolTrm = transform.Find("Symbol");
    }

    public override void UpdateUnit()
    {
        base.UpdateUnit();
        if (_symbolTrm is not null)
        {
            _symbolTrm.eulerAngles += new Vector3(0, _symbolRotateSpeed * Time.deltaTime, 0);
        }
    }

    public override void OnInteraction(ObjectUnit communicator, bool interactValue, params object[] param)
    {
        SceneControlManager.Instance.LoadScene(SceneType.Stage, () =>
        {
            StageManager.Instance.StartNewChapter(_data);
        });
    }
}