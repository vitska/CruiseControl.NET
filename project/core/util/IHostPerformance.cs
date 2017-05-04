namespace ThoughtWorks.CruiseControl.Core.Util
{
    public interface IHostPerformance
    {
        byte Cpu { get; }
        byte Memory { get; }
        byte Disk { get; }
    }
}
