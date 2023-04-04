namespace LiteDbLibrary.Schemas
{
    public class StringData : LiteDbObject
    {
        public string Value { get; set; }

        public override bool SameIndex(ILiteDbObject other)
        {
            return string.Equals(Value, (other as StringData)?.Value);
        }
    }
}
