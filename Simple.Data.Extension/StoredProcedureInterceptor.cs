﻿namespace Simple.Data.Extension
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Data;
	using System.Data.Common;
	using System.Dynamic;
	using Castle.DynamicProxy;
	using System.Reflection;

	/// <summary>
	/// The brain of the project: Windsor IoC Interceptor.
	/// </summary>
	public class StoredProcedureInterceptor : IInterceptor
	{
		private DbConnection connection;

		private DbDataReader reader;

		/// <summary>
		/// Main Intercetpr method.
		/// </summary>
		/// <param name="invocation"></param>
		public void Intercept(IInvocation invocation)
		{
			try
			{
				if (invocation.Method.DeclaringType == typeof(IDisposable))
				{
					this.connection.Dispose();
					return;
				}

				var factory = DbProviderFactories.GetFactory(ConfigurationManager.ConnectionStrings[Setup.connectionStringNames[invocation.Method.DeclaringType]].ProviderName);
				this.connection = factory.CreateConnection();
				connection.ConnectionString = ConfigurationManager.ConnectionStrings[Setup.connectionStringNames[invocation.Method.DeclaringType]].ConnectionString;
				connection.Open();

				bool returnAsResult = Attribute.IsDefined(invocation.Method, typeof(ReturnValueAsResultAttribute));

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
					
					//Check Nullable parameter. 
                    if (invocation.Arguments[i] == null && 
                        item.ParameterType.IsGenericType &&
                        item.ParameterType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                    { 
                        //Jump if ParameterValue == null && ParameterType is Nullable  
                    }
                    else
                    {
						var param = factory.CreateParameter();
						if (DBNameAttribute.IsDefined(item, typeof(DBNameAttribute)))
						{
							param.ParameterName = (DBNameAttribute.GetCustomAttribute(item, typeof(DBNameAttribute)) as DBNameAttribute).Name;
						}
						else
						{
							param.ParameterName = item.Name;
						}
						if (Attribute.IsDefined(item, typeof(ReturnValueAttribute)))
						{
							param.Direction = ParameterDirection.ReturnValue;
						}
						else
						{
							param.Direction = item.IsOut ? ParameterDirection.Output : ParameterDirection.Input;
						}
						
						//Check Nullable parameter && ParamValue != null
                        var type = item.ParameterType;
                        var underlyingType = Nullable.GetUnderlyingType(type);
                        var returnType = underlyingType ?? type;
                        param.DbType = (DbType)Enum.Parse(typeof(DbType), returnType.Name.Replace("&", ""));
						
						if (SizeAttribute.IsDefined(item, typeof(SizeAttribute)))
						{
							param.Size = (SizeAttribute.GetCustomAttribute(item, typeof(SizeAttribute)) as SizeAttribute).Size;
						}
						param.Value = invocation.Arguments[i];
						command.Parameters.Add(param);
					}
					i++;
				}
				if (invocation.Method.ReturnType != typeof(void))
				{
					if (!returnAsResult)
					{
						if (invocation.Method.ReturnType == typeof(Dictionary<string, object>))
						{
							this.reader = command.ExecuteReader();
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
						else if (invocation.Method.ReturnType.Namespace == "System" && invocation.Method.ReturnType != typeof(object))
						{
							var result = command.ExecuteScalar();
							if (result is DBNull)
								invocation.ReturnValue = null;
							else
								invocation.ReturnValue = command.ExecuteScalar();
						}
						else if (invocation.Method.ReturnType.GetInterface("IEnumerable") != null)
						{
							this.reader = command.ExecuteReader();
							Type type;
							if (invocation.Method.ReturnType.IsGenericType && invocation.Method.ReturnType.GetGenericArguments()[0] != typeof(object))
							{
								if (invocation.Method.ReturnType.GetGenericArguments()[0] == typeof(Dictionary<string, object>))
								{
									invocation.ReturnValue = Utils.GetIteratorDictionary(reader);
								}
								else
								{
									type = invocation.Method.ReturnType.GetGenericArguments()[0];
									invocation.ReturnValue = typeof(Utils).GetMethod("GetIterator", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(type).Invoke(null, new object[] { reader });
								}
							}
							else
							{
								invocation.ReturnValue = Utils.GetIteratorDynamic(reader);
							}

						}
						else if (invocation.Method.ReturnType == typeof(object))
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
							}
						}
					}
					else
					{
						var retval = factory.CreateParameter();

						retval.Direction = ParameterDirection.ReturnValue;
						retval.ParameterName = "RetVal";
						retval.DbType = (DbType)Enum.Parse(typeof(DbType), invocation.Method.ReturnType.Name.Replace("&", ""));
						command.Parameters.Add(retval);

						command.ExecuteNonQuery();

						invocation.ReturnValue = Convert.ChangeType(retval.Value, invocation.Method.ReturnType);						
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
			finally
			{
				if (invocation.Method.DeclaringType.GetInterface("IDisposable") == null) this.connection.Dispose();
			}
		}
	}
}
