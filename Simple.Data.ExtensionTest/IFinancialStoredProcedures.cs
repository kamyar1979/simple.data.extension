using Sama90.Model;
using System.Collections.Generic;
using System;
using Simple.Data.Extension;

namespace Simple.Data.ExtensionTest
{
	public interface IFinancialStoredProcedures
	{		
		IEnumerable<object> GetLessonList();
		Lesson GetLesson(int lessonCode);
		[ReturnValueAsResult]
		int TestRetVal();
		string TestRetValParam([DBName("param1")] string p, [ReturnValue] out int retval);
	}
}
