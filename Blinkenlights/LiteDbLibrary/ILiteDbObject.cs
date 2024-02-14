namespace LiteDbLibrary
{
    public interface ILiteDbObject
    {
        public int Id { get; set; }

        public bool SameIndex(ILiteDbObject other);
    }
}
