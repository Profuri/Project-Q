namespace AxisConvertSystem
{
    public interface IPassable
    {
        public bool PassableAfterAxis { get; set; }
        public void PassableCheck(AxisType axis);
    }
}