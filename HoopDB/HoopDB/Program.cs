namespace Blinkenlights.HoopDB
{
	public class Program
	{


        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSingleton<IDatabaseHandler, DatabaseHandler>();
            var app = builder.Build();

            app.MapGet("/read/{key}", (string key, IDatabaseHandler db) =>
            {
                var value = db.Read(key);
                if (string.IsNullOrWhiteSpace(value))
                {
                    return Results.NoContent();
                }

                return Results.Ok(value);
            });


            app.MapPut("/write/{key}", (string key, string value, IDatabaseHandler db) =>
            {
                if (db.Write(key, value))
                {
                    return Results.Ok();
                }
                else
                {
                    return Results.Problem();
                }
            });

            app.Run();
		}
	}
}