using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Castle.MicroKernel.Registration;
using System.Data.Common;
using System.Data.SqlClient;

namespace Simple.Data.Extension
{
	public static class Setup
	{
		internal static IWindsorContainer Container;
		internal static Dictionary<Type, string> connectionStringNames = new Dictionary<Type,string>();

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

		public static void Register<T>(string connectionName = "default") where T : class
		{
			Container.Register(Component.For<T>().Interceptors<StoredProcedureInterceptor>());
			connectionStringNames[typeof(T)] = connectionName;
		}

		public static T GetInstance<T>()
		{
			return Container.Resolve<T>();
		}

	}
}
