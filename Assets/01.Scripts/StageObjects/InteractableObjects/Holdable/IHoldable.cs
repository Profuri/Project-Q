using AxisConvertSystem;

public interface IHoldable
{
    public void Attach(ObjectHoldingHandler handler);
    public void Detach();
}
