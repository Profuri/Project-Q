namespace AxisConvertSystem
{
    public interface IStructureObject
    {
        public void Init(AxisConverter converter);
        public void ConvertDimension(AxisType axisType);
        public void TransformSynchronization(AxisType axisType);
        public void ObjectSetting();
    }
}