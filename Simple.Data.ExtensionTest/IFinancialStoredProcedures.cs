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
		string TestRetValParam(string param1, [ReturnValue] out int retval);
	}
}
