using LiteDB;

namespace LiteDbLibrary
{
	public class LiteDbHandler : ILiteDbHandler
	{
		public const string DATABASE_FILENAME = "BlinkenLights_LiteDb_Prod.db";

		private string DatabaseAbsoluteFilePath { get; init; }

		private Mutex Mutex { get; init; }

		public LiteDbHandler(string databaseAbsolutePath)
		{
			this.DatabaseAbsoluteFilePath = databaseAbsolutePath;
			this.Mutex = new Mutex();
		}

		public List<T> Read<T>() where T : ILiteDbObject
		{
			List<T> result = default;
			Mutex.WaitOne();
			using (var db = new LiteDatabase(this.DatabaseAbsoluteFilePath))
			{
				result = db.GetCollection<T>(typeof(T).Name)
					?.Query()
					?.ToList();
			}
			Mutex.ReleaseMutex();
			return result;
		}

		public void Overwrite<T>(T value) where T : ILiteDbObject
		{
			Mutex.WaitOne();
			using (var db = new LiteDatabase(this.DatabaseAbsoluteFilePath))
			{
				var col = db.GetCollection<T>(typeof(T).Name);
				col.DeleteAll();
				col.Insert(value);
			}
			Mutex.ReleaseMutex();
		}

		public void Upsert<T>(T value) where T : ILiteDbObject
		{
			Mutex.WaitOne();
			using (var db = new LiteDatabase(this.DatabaseAbsoluteFilePath))
			{
				var col = db.GetCollection<T>(typeof(T).Name);
				var existingValue = col.Query().ToList().FirstOrDefault(x => x.SameIndex(value));
				if (existingValue != null)
				{
					value.Id = existingValue.Id;
				}
				col.Upsert(value);
			}
			Mutex.ReleaseMutex();
		}

		public void Upsert<T>(List<T> values) where T : ILiteDbObject
		{
			values.ForEach(i => Upsert(i));
		}

		public void Delete<T>(T value) where T : ILiteDbObject
		{
			Mutex.WaitOne();
			using (var db = new LiteDatabase(this.DatabaseAbsoluteFilePath))
			{
				var col = db.GetCollection<T>(typeof(T).Name);
				var existingValue = col.Query().ToList().FirstOrDefault(x => x.Equals(value));
				if (existingValue != null)
				{
					col.Delete(existingValue.Id);
				}

				var col2 = db.GetCollection<T>(typeof(T).Name);
				if (col2.Query().Count() == 0)
				{
					db.DropCollection(typeof(T).Name);
				}
			}
			Mutex.ReleaseMutex();
		}

		private void DeleteRow(string key)
		{
			Mutex.WaitOne();
			using (var db = new LiteDatabase(this.DatabaseAbsoluteFilePath))
			{
				var col = db.GetCollection(key);
				col.DeleteAll();
				db.DropCollection(key);
			}
			Mutex.ReleaseMutex();
		}

		public void DeleteRow<T>()
		{
			DeleteRow(typeof(T).Name);
		}

		public List<string> ReadAll()
		{
			List<string> results;
			Mutex.WaitOne();
			using (var db = new LiteDatabase(this.DatabaseAbsoluteFilePath))
			{
				results = db.GetCollectionNames().ToList();
			}
			Mutex.ReleaseMutex();
			return results;
		}

		public Dictionary<string, List<BsonDocument>> ReadFull()
		{
			Dictionary<string, List<BsonDocument>> results = new();
			Mutex.WaitOne();
			using (var db = new LiteDatabase(this.DatabaseAbsoluteFilePath))
			{
				db.GetCollectionNames().ToList().ForEach(k =>
				{
					results.Add(k, db.GetCollection(k).Query().ToList());
				});
			}
			Mutex.ReleaseMutex();
			return results;
		}

		public void Clear()
		{
			ReadAll().ForEach(i => DeleteRow(i));
		}
	}
}
