namespace System.Dynamic
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Reflection.Emit;

	/// <summary>
	/// This class creates a dynamic type on-the-fly.
	/// </summary>
	public class DynamicTypeBuilder
	{
		private AssemblyBuilder assemblyBuilder;
		private ModuleBuilder moduleBuilder;
		private TypeBuilder typeBuilder;

		/// <summary>
		/// Creates a new type using the name.
		/// </summary>
		/// <param name="typeName">The name of the type, containing the possible namespace.</param>
		public DynamicTypeBuilder(string typeName)
		{
			this.assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("OnTheFly"), AssemblyBuilderAccess.Run);
			this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule("AnonymousTypes");
			this.typeBuilder = this.moduleBuilder.DefineType(typeName, TypeAttributes.Public);
		}

		/// <summary>
		/// Creates a new type using the name and the module name.
		/// </summary>
		/// <param name="typeName">The name of the type, containing the possible namespace.</param>
		/// <param name="moduleName">The name fo the module.</param>
		public DynamicTypeBuilder(string typeName, string moduleName)
		{
			this.assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("OnTheFly"), AssemblyBuilderAccess.Run);
			this.moduleBuilder = this.assemblyBuilder.DefineDynamicModule(moduleName);
			this.typeBuilder = this.moduleBuilder.DefineType(typeName, TypeAttributes.Public);
		}

		/// <summary>
		/// Creates a new instance of this class using a sample dicitonary, containig the properties and their values.
		/// </summary>
		/// <param name="dict">The Dictionary contains the property names as key, and their values as values.</param>
		/// <returns>An instance of this class.</returns>
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

		/// <summary>
		/// Adds a property with Type and name.
		/// </summary>
		/// <typeparam name="T">The type of the property.</typeparam>
		/// <param name="name">Name of the property.</param>
		/// <returns>Returns the propertyInfo object for possible using.</returns>
		public PropertyInfo AddProperty<T>(string name)
		{
			return this.AddProperty(name, typeof(T));
		}

		/// <summary>
		/// Adds a property with the type and name.
		/// </summary>
		/// <param name="name">Name of the property</param>
		/// <param name="type">Type of the property.</param>
		/// <returns>Returns the propertyInfo object for possible using.</returns>
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

		/// <summary>
		/// Returns the resulting type.
		/// </summary>
		/// <remarks>
		///  Note that you can not add any futher porperties, due to C# limitations.That is, you can not call AddProperty methods after calling this.
		/// </remarks>
		/// <returns>Returns the Type as variable.</returns>
		public Type CreateType()
		{
			return this.typeBuilder.CreateType();
		}
	}
}