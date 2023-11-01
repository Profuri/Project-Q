using UnityEngine;

namespace StageStructureConvertSystem
{
    public interface IStructureObject
    {
        public void Init();
        public void TransformSynchronization();
        public void ApplyCollider(EAxisType axisType);
        public void ObjectSetting(ObjectInfo info);
    }
}