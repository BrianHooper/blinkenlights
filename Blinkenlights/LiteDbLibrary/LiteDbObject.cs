namespace LiteDbLibrary
{
    public abstract class LiteDbObject : ILiteDbObject
    {
        public virtual int Id { get; set; }

        public virtual bool SameIndex(ILiteDbObject other) => this.Equals(other);
    }
}
