namespace AxisConvertSystem
{
    public interface IPassable
    {
        public bool PassableLastAxis { get; set; }
        public bool PassableAfterAxis { get; set; }
        public void PassableCheck(AxisType axis);
        public bool IsPassableLastAxis();
    }
}