namespace AKB.Common.Data
{
    public interface IAbstractFactory
    {
        IDbHelper GetDbHelper(string dbName);
    }
}
