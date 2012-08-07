namespace System.Dynamic
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;

	public class DynamicTypeBuilder
	{
		private AssemblyBuilder assemblyBuilder;
		private ModuleBuilder moduleBuilder;
		private TypeBuilder typeBuilder;

		public DynamicTypeBuilder(string typeName)
		{
			this.assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("OnTheFly"), AssemblyBuilderAccess.Run);
			this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule("AnonymousTypes");
			this.typeBuilder = this.moduleBuilder.DefineType(typeName, TypeAttributes.Public);
		}

		public DynamicTypeBuilder(string typeName, string moduleName)
		{
			this.assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("OnTheFly"), AssemblyBuilderAccess.Run);
			this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(moduleName);
			this.typeBuilder = this.moduleBuilder.DefineType(typeName, TypeAttributes.Public);
		}

		public static DynamicTypeBuilder FromDictionary(IDictionary<string, object> dict)
		{
			string name = "anonym_" + dict.GetHashCode().ToString();
			var result = new DynamicTypeBuilder(name);
			foreach (var item in dict)
			{
				result.AddProperty(item.Key, item.Value.GetType());
			}
			return result;
		}

		public PropertyInfo AddProperty<T>(string name)
		{
			return this.AddProperty(name, typeof(T));
		}

		public PropertyInfo AddProperty(string name, Type type)
		{
			var fieldBuilder = typeBuilder.DefineField('_' + name.ToLower(), type, FieldAttributes.Private | FieldAttributes.HasDefault);
			var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, type, Type.EmptyTypes);
			var methodGetBuilder = typeBuilder.DefineMethod("get_" + name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, type, Type.EmptyTypes);
			var methodSetBuilder = typeBuilder.DefineMethod("set_" + name, MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, null, new Type[] { type });

			ILGenerator getIL = methodGetBuilder.GetILGenerator();

			getIL.Emit(OpCodes.Ldarg_0);
			getIL.Emit(OpCodes.Ldfld, fieldBuilder);
			getIL.Emit(OpCodes.Ret);

			ILGenerator setIL = methodSetBuilder.GetILGenerator();

			setIL.Emit(OpCodes.Ldarg_0);
			setIL.Emit(OpCodes.Ldarg_1);
			setIL.Emit(OpCodes.Stfld, fieldBuilder);
			setIL.Emit(OpCodes.Ret);

			propertyBuilder.SetGetMethod(methodGetBuilder);
			propertyBuilder.SetSetMethod(methodSetBuilder);

			return propertyBuilder;
		}

		public Type CreateType()
		{
			return this.typeBuilder.CreateType();
		}
	}
}