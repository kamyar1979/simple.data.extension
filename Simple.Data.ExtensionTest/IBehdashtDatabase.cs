using System;
using System.Collections.Generic;
namespace SamaWebAPI
{
	public interface IBehdashtDatabase : IDisposable
	{
		int? DasGetStTermStatus(
			string stno,
			string termCode);

		IEnumerable<object> DasBehdashtGetInfoStudents(string StNo);//string NationalCode, 
	}
}