using LiteDbLibrary;

namespace Blinkenlights.Data.LiteDb
{
    public static class LiteDbFactory
    {
        public static LiteDbHandler Build(IWebHostEnvironment environment)
        {
            var pathParts = new string[] { environment.WebRootPath, LiteDbHandler.DATABASE_FILENAME };
            var databaseAbsoluteFilePath = Path.Combine(pathParts);
            return new LiteDbHandler(databaseAbsoluteFilePath);
        }
    }
}
