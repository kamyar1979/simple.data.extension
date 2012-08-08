namespace Simple.Data.Extension
{
	using System;

	/// <summary>
	/// says that the target stored procedure returns the result as 'Return Value' parameter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class ReturnValueAsResultAttribute : Attribute { }

	/// <summary>
	/// says that this methid parameter should be considered as target stored procedure 'Return Value' paramter.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public class ReturnValueAttribute : Attribute { }

}
