namespace StageStructureConvertSystem
{
    public interface IStructureObject
    {
        public void Init(StructureConverter converter);
        public void ConvertDimension(EAxisType axisType);
        public void TransformSynchronization(EAxisType axisType);
        public void ObjectSetting();
    }
}