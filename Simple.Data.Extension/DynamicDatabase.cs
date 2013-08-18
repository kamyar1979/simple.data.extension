namespace Simple.Data.Extension
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.Common;
	using System.Dynamic;

	/// <summary>
	/// This is a dynamic class representing a psedue database which its stored procedures is callable via method names.
	/// </summary>
	public class DynamicDatabase : DynamicObject
	{
		private DbConnection connection;

		private DbDataReader reader;

		internal string ConnectionStringName { get; set; }

		/// <summary>
		/// This is Microstf's standard way for captureing any method call in a dynamic object.
		/// </summary>
		/// <param name="binder"></param>
		/// <param name="args"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
		{
			result = null;

			var factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ProviderName);
			connection = factory.CreateConnection();
			connection.ConnectionString = ConfigurationManager.ConnectionStrings[this.ConnectionStringName].ConnectionString;
			connection.Open();

			string methodName = binder.Name;
			bool returnAsResult = methodName.EndsWith("_");
			if (methodName.EndsWith("_"))
			{
				methodName = methodName.Substring(0, methodName.Length - 1);
			}

			var command = factory.CreateCommand();
			command.Connection = this.connection;
			command.CommandType = CommandType.StoredProcedure;
			command.CommandText = methodName;

			int i = 0;

			if (binder.CallInfo.ArgumentNames.Count == 0)
			{
				var builder = factory.CreateCommandBuilder();
				builder.GetType().GetMethod("DeriveParameters").Invoke(null, new[] { command });

				for (i = 0; i < binder.CallInfo.ArgumentCount; i++)
				{
					command.Parameters[i + 1].Value = args[i];
				}
			}
			else
			{
				foreach (var item in binder.CallInfo.ArgumentNames)
				{
					var param = factory.CreateParameter();
					param.ParameterName = item;
					param.Direction = ParameterDirection.Input;
					param.DbType = (DbType)Enum.Parse(typeof(DbType), args[i].GetType().Name);
					param.Value = args[i];
					command.Parameters.Add(param);
					i++;
				}
			}
			
			Type returnType;
			if (binder.GetGenericTypeArguments().Count > 0)
				returnType = binder.GetGenericTypeArguments()[0];
			else
				returnType = typeof(object);

			if (returnType != typeof(void))
			{
				if (!returnAsResult)
				{
					if (returnType == typeof(Dictionary<string, object>))
					{
						var reader = command.ExecuteReader();
						if (reader.Read())
						{
							string[] fields = new string[reader.FieldCount];
							for (i = 0; i < reader.FieldCount; i++)
							{
								fields[i] = reader.GetName(i);
							}

							if (returnType == typeof(object))
							{
								var instance = new Dictionary<string, object>();
								foreach (var name in fields)
								{
									instance.Add(name, reader[name] is DBNull ? null : reader[name]);
								}
								result = instance;
							}
						}
					}
					else if (returnType.IsPrimitive || returnType == typeof(string))
					{
						result = command.ExecuteScalar();
					}
					else if (returnType.GetInterface("IEnumerable") != null)
					{
						this.reader = command.ExecuteReader();
						Type type;
						if (returnType.IsGenericType && returnType.GetGenericArguments()[0] != typeof(object))
						{
							if (returnType.GetGenericArguments()[0] == typeof(Dictionary<string, object>))
							{
								result = Utils.GetIteratorDictionary(reader);
							}
							else
							{
								type = returnType.GetGenericArguments()[0];
								result = this.GetType().GetMethod("GetIterator").MakeGenericMethod(type).Invoke(this, new object[] { reader });
							}
						}
						else
						{
							result = Utils.GetIteratorDynamic(reader);
						}

					}
					else if (returnType == typeof(object))
					{
						using (this.reader = command.ExecuteReader())
						{
							if (reader.Read())
							{
								string[] fields = new string[reader.FieldCount];
								for (i = 0; i < reader.FieldCount; i++)
								{
									fields[i] = reader.GetName(i);
								}

								if (returnType == typeof(object))
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
									result = instance;
								}
								else
								{
									var instance = Activator.CreateInstance(returnType);
									foreach (var prop in returnType.GetProperties())
									{
										if (Array.IndexOf(fields, prop.Name) != -1)
										{
											prop.SetValue(instance, reader[prop.Name] is DBNull ? null : reader[prop.Name], null);
										}
									}
									result = instance;
								}
							}
						}
					}
				}
				else
				{
					var retval = factory.CreateParameter();

					retval.Direction = ParameterDirection.ReturnValue;
					retval.ParameterName = "RetVal";
					retval.DbType = (DbType)Enum.Parse(typeof(DbType), returnType.Name);
					command.Parameters.Add(retval);

					command.ExecuteNonQuery();

					result = retval.Value;
				}
			}
			else
			{
				command.ExecuteNonQuery();
			}
			return true;
		}
	}
}
