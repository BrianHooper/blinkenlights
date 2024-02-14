namespace LiteDbLibrary
{
    public interface ILiteDbHandler
    {
        public void Delete<T>(T value) where T : ILiteDbObject;

        public void DeleteRow<T>();

        List<T> Read<T>() where T : ILiteDbObject;

        void Overwrite<T>(T value) where T : ILiteDbObject;

        void Upsert<T>(T value) where T : ILiteDbObject;

        void Upsert<T>(List<T> values) where T : ILiteDbObject;

        public List<string> ReadAll();

        public void Clear();
    }
}
