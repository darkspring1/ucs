namespace Rkl.DAL.Abstractions
{
    public interface IPaging
    {
        int Skip { get; }
        int Take { get; }
    }
}
