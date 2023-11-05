namespace StageStructureConvertSystem
{
    public interface IStructureObject
    {
        public void Init();
        public void ConvertDimension(EAxisType axisType);
        public void TransformSynchronization(EAxisType axisType);
        public void ObjectSetting();
    }
}