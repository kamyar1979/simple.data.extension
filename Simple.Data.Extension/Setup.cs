namespace Simple.Data.Extension
{
	using System;
	using System.Collections.Generic;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor;

	/// <summary>
	/// This class is the only thing you have to consider using: Sets up the whole things!
	/// </summary>
	public static class Setup
	{
		internal static IWindsorContainer Container;
		internal static Dictionary<Type, string> connectionStringNames = new Dictionary<Type,string>();

		/// <summary>
		/// Initializes the Simple.Data.Extension package.
		/// </summary>
		public static void Initialize()
		{
			if (Container == null)
			{				
				Container = new WindsorContainer()
					.Register(
					Component.For<StoredProcedureInterceptor>()
					);
			}
		}

		/// <summary>
		/// Registers an interface type and binds it to a connection string name in teh configuration file.
		/// </summary>
		/// <typeparam name="T">Interface type</typeparam>
		/// <param name="connectionName">Database connection string name  in the web/app.config</param>
		public static void Register<T>(string connectionName = "default") where T : class
		{
			Container.Register(Component.For<T>().Interceptors<StoredProcedureInterceptor>());
			connectionStringNames[typeof(T)] = connectionName;
		}

		/// <summary>
		/// Creates an instance of the DB provider to use the stored procedures.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public static T GetInstance<T>() where T: class
		{
			return Container.Resolve<T>();
		}

	}
}
