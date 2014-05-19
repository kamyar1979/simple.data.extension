namespace Simple.Data.Extension
{
	using System;
	using System.Collections.Generic;
	using Castle.DynamicProxy;

	/// <summary>
	/// This class is the only thing you have to consider using: Sets up the whole things!
	/// </summary>
	public static class Setup
	{
		internal static StoredProcedureInterceptor interceptor;
		internal static Dictionary<Type, string> connectionStringNames = new Dictionary<Type, string>();		

		static Setup()
		{
			interceptor = new StoredProcedureInterceptor();
		}

		/// <summary>
		/// Registers an interface type and binds it to a connection string name in teh configuration file.
		/// </summary>
		/// <typeparam name="T">Interface type</typeparam>
		/// <param name="connectionName">Database connection string name  in the web/app.config</param>
		public static void Register<T>(string connectionName = "default") where T : class
		{
			connectionStringNames[typeof(T)] = connectionName;
		}

		/// <summary>
		/// Creates an instance of the DB provider to use the stored procedures.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetInstance<T>() where T : class
		{
			var proxyGen = new ProxyGenerator();
			return proxyGen.CreateInterfaceProxyWithoutTarget<T>(new StoredProcedureInterceptor());
		}

		/// <summary>
		/// Creates a dynamic instance of the DB provider to use the stored procedures.
		/// </summary>
		/// <param name="connectionStringName">Name of the connection string which must be used as database endpoint.</param>
		/// <returns></returns>
		public static object GetInstance(string connectionStringName)
		{
			return new DynamicDatabase() { ConnectionStringName = connectionStringName };
		}

	}
}
