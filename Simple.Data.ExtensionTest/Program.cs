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
			Setup.Initialize();
			Setup.Register<IFinancialStoredProcedures>("Financial");

			var financial = Setup.GetInstance<IFinancialStoredProcedures>();

			var res = financial.GetLessonList();			

			foreach (dynamic item in res)
			{
				Console.WriteLine(item.LessonName);
			}
			var lesson = financial.GetLesson(1001251);

			Console.WriteLine(lesson.Unit);
			Console.ReadLine();

		}
	}
}
