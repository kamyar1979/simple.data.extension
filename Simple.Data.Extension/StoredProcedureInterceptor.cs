namespace Simple.Data.Extension
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.Common;
	using Castle.DynamicProxy;
	using System.Dynamic;
	using System.Linq.Expressions;

	public class StoredProcedureInterceptor : IInterceptor
	{
		private DbConnection connection;

		public void Intercept(IInvocation invocation)
		{
			var factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[Setup.connectionStringNames[invocation.Method.DeclaringType]].ProviderName);
			connection = factory.CreateConnection();
			connection.ConnectionString = ConfigurationManager.ConnectionStrings[Setup.connectionStringNames[invocation.Method.DeclaringType]].ConnectionString;
			connection.Open();

			var command = factory.CreateCommand();
			command.Connection = this.connection;
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = invocation.Method.Name;
			int i = 0;
			foreach (var item in invocation.Method.GetParameters())
			{
				if (item.IsOptional && invocation.Arguments.Length < i + 1)
				{
					break;
				}
				var param = factory.CreateParameter();
				param.ParameterName = item.Name;
				param.Direction = item.IsOut ? ParameterDirection.Output : ParameterDirection.Input;
				param.DbType = (DbType)Enum.Parse(typeof(DbType), item.ParameterType.Name.Replace("&", ""));
				param.Value = invocation.Arguments[i];
				command.Parameters.Add(param);
				i++;
			}
			if (invocation.Method.ReturnType != null)
			{
				if (invocation.Method.ReturnType == typeof(Dictionary<string, object>))
				{
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						string[] fields = new string[reader.FieldCount];
						for (i = 0; i < reader.FieldCount; i++)
						{
							fields[i] = reader.GetName(i);
						}

						if (invocation.Method.ReturnType == typeof(object))
						{
							var instance = new Dictionary<string, object>();
							foreach (var name in fields)
							{
								instance.Add(name, reader[name] is DBNull ? null : reader[name]);
							}
							invocation.ReturnValue = instance;
						}
					}

				}
				else if (invocation.Method.ReturnType.GetInterface("IEnumerable") != null)
				{
					var reader = command.ExecuteReader();
					Type type;
					if (invocation.Method.ReturnType.IsGenericType && invocation.Method.ReturnType.GetGenericArguments()[0] != typeof(object))
					{
						if (invocation.Method.ReturnType.GetGenericArguments()[0] == typeof(Dictionary<string, object>))
						{
							invocation.ReturnValue = this.GetIteratorDictionary(reader);
						}
						else
						{
							type = invocation.Method.ReturnType.GetGenericArguments()[0];
							invocation.ReturnValue = this.GetType().GetMethod("GetIterator").MakeGenericMethod(type).Invoke(this, new object[] { reader });
						}
					}
					else
					{
						invocation.ReturnValue = this.GetIteratorDynamic(reader);
					}

				}
				else if (invocation.Method.ReturnType.IsPrimitive || invocation.Method.ReturnType == typeof(string))
				{
					invocation.ReturnValue = command.ExecuteScalar();
				}
				else
				{
					using (var reader = command.ExecuteReader())
					{
						if (reader.Read())
						{
							string[] fields = new string[reader.FieldCount];
							for (i = 0; i < reader.FieldCount; i++)
							{
								fields[i] = reader.GetName(i);
							}

							if (invocation.Method.ReturnType == typeof(object))
							{
								var builder = new DynamicTypeBuilder("anonym_" + reader.GetHashCode());
								foreach (var name in fields)
								{
									builder.AddProperty(name, reader.GetFieldType(reader.GetOrdinal(name)));
								}
								var type = builder.CreateType();
								var instance = Activator.CreateInstance(type);
								foreach (var name in fields)
								{
									var fieldType = reader.GetFieldType(reader.GetOrdinal(name));
									type.GetProperty(name).SetValue(instance, reader[name] is DBNull ? null : reader[name], null);
								}
								invocation.ReturnValue = instance;
							}
							else
							{
								var instance = Activator.CreateInstance(invocation.Method.ReturnType);
								foreach (var prop in invocation.Method.ReturnType.GetProperties())
								{
									if (Array.IndexOf(fields, prop.Name) != -1)
									{
										prop.SetValue(instance, reader[prop.Name] is DBNull ? null : reader[prop.Name], null);
									}
								}
								invocation.ReturnValue = instance;
							}
						}
						else
						{
							invocation.ReturnValue = null;
						}
					}
				}
			}
			else
			{
				command.ExecuteNonQuery();
			}
			i = 0;
			foreach (var item in invocation.Method.GetParameters())
			{
				if (item.IsOut)
				{
					invocation.Arguments[i] = command.Parameters[i].Value;
				}
				i++;
			}
		}

		private IEnumerable<Dictionary<string, object>> GetIteratorDictionary(DbDataReader reader)
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

		public IEnumerable<T> GetIterator<T>(IDataReader reader) where T : class, new()
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

		public IEnumerable<object> GetIteratorDynamic(IDataReader reader)
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
