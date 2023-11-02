using UnityEngine;

namespace StageStructureConvertSystem
{
    public interface IStructureObject
    {
        public void Init();
        public void ConvertDimension(EAxisType axisType);
        public void ConvertMesh(EAxisType axisType);
        public void TransformSynchronization(EAxisType axisType);
        public void ApplyCollider(EAxisType axisType);
        public void ObjectSetting(ObjectInfo info);
    }
}