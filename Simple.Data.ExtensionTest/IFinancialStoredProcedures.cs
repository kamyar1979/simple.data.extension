using Sama90.Model;
using System.Collections.Generic;
using System;
using Simple.Data.Extension;

namespace Simple.Data.ExtensionTest
{
	public interface IFinancialStoredProcedures : IDisposable
	{
		IEnumerable<object> GetLessonList();
		IEnumerable<Organization> TestSP2(int p1);
		Lesson GetLesson(int lessonCode);
		[ReturnValueAsResult]
		bool TestRetVal();
		string TestRetValParam([DBName("param1")] string p, [ReturnValue] out int retval);
	}
}
