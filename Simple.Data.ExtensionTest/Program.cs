﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Simple.Data.Extension;
using System.Linq.Expressions;

namespace Simple.Data.ExtensionTest
{
	class Program
	{
		static void Main(string[] args)
		{			
			Setup.Register<IFinancialStoredProcedures>("Financial");

			//var db = Setup.GetInstance<IFinancialStoredProcedures>();

			//var res = financial.GetLessonList();			

			//foreach (dynamic item in res)
			//{
			//    Console.WriteLine(item.LessonName);
			//}
			//var lesson = financial.GetLesson(1001251);

			//Console.WriteLine(lesson.Unit);

			//Console.WriteLine(db.TestRetVal());

			//int retval = 0;						

			var db = Setup.GetInstance<IFinancialStoredProcedures>();

			//int retval = db.TestRetVal<int>();			
			//bool retval = db.TestRetVal();
			//IEnumerable<object> ret = db.TestSP2<IEnumerable<object>>(1);
			var retval = db.TestSP2(1);

			Console.Write(retval.ToList()[0].OrgName);

			//var list = ret.ToList();

			//Console.WriteLine(db.TestRetValParam("Kamyar", out retval));
			//Console.WriteLine(retval);

			Console.ReadLine();

		}
	}
}
