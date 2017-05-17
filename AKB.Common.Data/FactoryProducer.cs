namespace AKB.Common.Data
{
    public class FactoryProducer
    {
        public static IAbstractFactory GetFactory(string type)
        {
            if (string.IsNullOrEmpty(type)) return null;

            switch (type.ToUpper())
            {
                case "DB": return new DbFactory();
                default: return null;
            }
        }
    }
}
