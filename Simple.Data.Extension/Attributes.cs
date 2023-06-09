﻿namespace Simple.Data.Extension
{
	using System;
	using System.Text.RegularExpressions;

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

    /// <summary>
    /// says that this methid parameter should be considered as target stored procedure 'Size' parameter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public class SizeAttribute : Attribute
    { 
        /// <summary>
		/// Initializes a new instance of the attribute.
		/// </summary>
		/// <param name="size">Size of the string</param>
        public SizeAttribute(int size)
		{
			this.Size = size;
		}

		/// <summary>
		/// The parameter size in the database stored procedure syntax.
		/// </summary>
		public int Size { get; set; }
    }
	
	/// <summary>
	/// This attributes is used when you want to name the attribute something other than the database name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Parameter)]
	public class DBNameAttribute : Attribute
	{
		/// <summary>
		/// Initializes a new instance of the attribute.
		/// </summary>
		/// <param name="name"></param>
		public DBNameAttribute(string name)
		{
			if (string.IsNullOrEmpty(name) || !Regex.IsMatch(name, @"^\w*$"))
				throw new ArgumentException("The name must be an valid alphanumeric variable name.");
			this.Name = name;
		}

		/// <summary>
		/// The parameter name in the database stored procedure syntax.
		/// </summary>
		public string Name { get; set; }
	}

}
