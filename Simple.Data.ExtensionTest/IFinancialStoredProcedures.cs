using Sama90.Model;
using System.Collections.Generic;
using System;

namespace Simple.Data.ExtensionTest
{
	public interface IFinancialStoredProcedures
	{		
		IEnumerable<object> GetLessonList();
		Lesson GetLesson(int lessonCode);
	}
}
