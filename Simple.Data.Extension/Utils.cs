namespace Simple.Data.Extension
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.Common;
	using System.Dynamic;

	internal static class Utils
	{
		internal static IEnumerable<Dictionary<string, object>> GetIteratorDictionary(DbDataReader reader)
		{
			Dictionary<string, object> instance;
			string[] fields = new string[reader.FieldCount];
			for (int i = 0; i < reader.FieldCount; i++)
			{
				fields[i] = reader.GetName(i);
			}
			while (reader.Read())
			{
				instance = new Dictionary<string, object>();
				foreach (var name in fields)
				{
					instance.Add(name, reader[name] is DBNull ? null : reader[name]);
				}
				yield return instance;
			}
			reader.Close();
			yield break;
		}

		internal static IEnumerable<T> GetIterator<T>(IDataReader reader) where T : class, new()
		{
			T instance;
			string[] fields = new string[reader.FieldCount];
			for (int i = 0; i < reader.FieldCount; i++)
			{
				fields[i] = reader.GetName(i);
			}
			while (reader.Read())
			{
				instance = new T();
				foreach (var prop in typeof(T).GetProperties())
				{
					if (Array.IndexOf(fields, prop.Name) != -1)
					{
						prop.SetValue(instance, reader[prop.Name] is DBNull ? null : reader[prop.Name], null);
					}
				}
				yield return instance;
			}
			reader.Close();
			yield break;
		}

		internal static IEnumerable<object> GetIteratorDynamic(IDataReader reader)
		{
			var builder = new DynamicTypeBuilder("anonym_" + reader.GetHashCode());
			string[] fields = new string[reader.FieldCount];
			for (int i = 0; i < reader.FieldCount; i++)
			{
				fields[i] = reader.GetName(i);
			}
			foreach (var name in fields)
			{
				builder.AddProperty(name, reader.GetFieldType(reader.GetOrdinal(name)));
			}
			var type = builder.CreateType();
			while (reader.Read())
			{
				var instance = Activator.CreateInstance(type);
				foreach (var name in fields)
				{
					var fieldType = reader.GetFieldType(reader.GetOrdinal(name));
					type.GetProperty(name).SetValue(instance, reader[name] is DBNull ? null : reader[name], null);
				}
				yield return instance;
			}
			reader.Close();
			yield break;
		}

	}
}
