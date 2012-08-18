namespace Simple.Data.Extension
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.Common;
	using System.Dynamic;
	using LinFu.DynamicProxy;

	/// <summary>
	/// The brain of the project: Windsor IoC Interceptor.
	/// </summary>
	public class StoredProcedureInterceptor : IInterceptor
	{
		private DbConnection connection;

		/// <summary>
		/// Main Intercetpr method.
		/// </summary>
		/// <param name="info"></param>
		public object Intercept(InvocationInfo info)
		{
			var factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[Setup.connectionStringNames[info.TargetMethod.DeclaringType]].ProviderName);
			connection = factory.CreateConnection();
			connection.ConnectionString = ConfigurationManager.ConnectionStrings[Setup.connectionStringNames[info.TargetMethod.DeclaringType]].ConnectionString;
			connection.Open();

			bool returnAsResult = Attribute.IsDefined(info.TargetMethod, typeof(ReturnValueAsResultAttribute));

			var command = factory.CreateCommand();
			command.Connection = this.connection;
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = info.TargetMethod.Name;
			int i = 0;
			foreach (var item in info.TargetMethod.GetParameters())
			{
				if (item.IsOptional && info.Arguments.Length < i + 1)
				{
					break;
				}
				var param = factory.CreateParameter();
				param.ParameterName = item.Name;
				if (Attribute.IsDefined(item, typeof(ReturnValueAttribute)))
				{
					param.Direction = ParameterDirection.ReturnValue;
				}
				else
				{
					param.Direction = item.IsOut ? ParameterDirection.Output : ParameterDirection.Input;
				}
				param.DbType = (DbType)Enum.Parse(typeof(DbType), item.ParameterType.Name.Replace("&", ""));
				param.Value = info.Arguments[i];
				command.Parameters.Add(param);
				i++;
			}
			if (info.TargetMethod.ReturnType != null)
			{
				if (!returnAsResult)
				{
					if (info.TargetMethod.ReturnType == typeof(Dictionary<string, object>))
					{
						var reader = command.ExecuteReader();
						if (reader.Read())
						{
							string[] fields = new string[reader.FieldCount];
							for (i = 0; i < reader.FieldCount; i++)
							{
								fields[i] = reader.GetName(i);
							}

							if (info.TargetMethod.ReturnType == typeof(object))
							{
								var instance = new Dictionary<string, object>();
								foreach (var name in fields)
								{
									instance.Add(name, reader[name] is DBNull ? null : reader[name]);
								}
								return instance;
							}
						}
					}
					else if (info.TargetMethod.ReturnType.IsPrimitive || info.TargetMethod.ReturnType == typeof(string))
					{
						var result = command.ExecuteScalar();
						int j = 0;
						foreach (var item in info.TargetMethod.GetParameters())
						{
							if (Attribute.IsDefined(item, typeof(ReturnValueAttribute)))
							{
								info.Arguments[j] = command.Parameters[item.Name].Value;
							}
							j++;
						}
						return result;
					}
					else if (info.TargetMethod.ReturnType.GetInterface("IEnumerable") != null)
					{
						var reader = command.ExecuteReader();
						Type type;
						if (info.TargetMethod.ReturnType.IsGenericType && info.TargetMethod.ReturnType.GetGenericArguments()[0] != typeof(object))
						{
							if (info.TargetMethod.ReturnType.GetGenericArguments()[0] == typeof(Dictionary<string, object>))
							{
								return this.GetIteratorDictionary(reader);
							}
							else
							{
								type = info.TargetMethod.ReturnType.GetGenericArguments()[0];
								return this.GetType().GetMethod("GetIterator").MakeGenericMethod(type).Invoke(this, new object[] { reader });
							}
						}
						else
						{
							return this.GetIteratorDynamic(reader);
						}

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

								if (info.TargetMethod.ReturnType == typeof(object))
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
									return instance;
								}
								else
								{
									var instance = Activator.CreateInstance(info.TargetMethod.ReturnType);
									foreach (var prop in info.TargetMethod.ReturnType.GetProperties())
									{
										if (Array.IndexOf(fields, prop.Name) != -1)
										{
											prop.SetValue(instance, reader[prop.Name] is DBNull ? null : reader[prop.Name], null);
										}
									}
									return instance;
								}
							}
							else
							{
								return null;
							}
						}
					}
				}
				else
				{
					var retval = factory.CreateParameter();

					retval.Direction = ParameterDirection.ReturnValue;
					retval.ParameterName = "RetVal";
					retval.DbType = (DbType)Enum.Parse(typeof(DbType), info.TargetMethod.ReturnType.Name.Replace("&", ""));
					command.Parameters.Add(retval);

					command.ExecuteNonQuery();

					return retval.Value;
				}
			}
			else
			{
				command.ExecuteNonQuery();
			}
			i = 0;
			foreach (var item in info.TargetMethod.GetParameters())
			{
				if (item.IsOut)
				{
					info.Arguments[i] = command.Parameters[i].Value;
				}
				i++;
			}
			return null;
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

		private IEnumerable<T> GetIterator<T>(IDataReader reader) where T : class, new()
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

		private IEnumerable<object> GetIteratorDynamic(IDataReader reader)
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
