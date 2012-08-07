
namespace Sama90.Model
{
	using System;	

	/// <summary>
	/// اطلاعات ثابت دروس
	/// </summary>
	[Serializable]
	public partial class Lesson
	{
		/// <summary>
		/// Initializes a new instance of the class
		/// </summary>
		public Lesson()
		{
			DefaultAcceptGradeStateId = 0;
			DefaultRejectGradeStateId = 0;
			FinancialTypeId = 0;
			GradePassedTypeId = 1;
			HasActive = 1;
			IncompleteGradeTermLimit = 0;
			MinPassedGrade = 10;
			PracticalMinGrade = 10;
			TheoreticalMinGrade = 10;
		}

		/// <summary>
		/// دروس هم نياز
		/// </summary>
		public virtual string CoPrerequisite
		{
			get;
			set;
		}

		/// <summary>
		/// دروس هم ارز/متضاد
		/// </summary>
		public virtual string ConflictLesson
		{
			get;
			set;
		}

		/// <summary>
		/// وضعيت نمره پيش فرض قبولي
		/// </summary>
		public virtual int? DefaultAcceptGradeStateId
		{
			get;
			set;
		}

		/// <summary>
		/// وضعيت نمره پيش فرض مردودي
		/// </summary>
		public virtual int? DefaultRejectGradeStateId
		{
			get;
			set;
		}

		/// <summary>
		/// وضعيت انتخاب واحد پيش فرض
		/// </summary>
		public virtual int? DefualtLessonStatusId
		{
			get;
			set;
		}

		/// <summary>
		/// کد زيردرس
		/// </summary>
		public virtual string DetailLessonCode
		{
			get;
			set;
		}

		/// <summary>
		/// نام انگليسي
		/// </summary>
		public virtual string EnglishName
		{
			get;
			set;
		}

		/// <summary>
		/// کد دانشکده
		/// </summary>
		public virtual int? FacultyId
		{
			get;
			set;
		}

		/// <summary>
		/// وضعيت مالي درس
		/// </summary>
		public virtual int? FinancialTypeId
		{
			get;
			set;
		}

		/// <summary>
		/// نوع قبولی نمره
		/// </summary>
		public virtual int GradePassedTypeId
		{
			get;
			set;
		}

		/// <summary>
		/// فعال است
		/// </summary>
		public virtual int HasActive
		{
			get;
			set;
		}

		/// <summary>
		/// تعداد نیمسال نمره ناتمام
		/// </summary>
		public virtual int IncompleteGradeTermLimit
		{
			get;
			set;
		}

		/// <summary>
		/// کد نوع دروس اسلامي
		/// </summary>
		public virtual int? IslamicLessonTypeId
		{
			get;
			set;
		}

		/// <summary>
		/// تاريخ و زمان آخرين تغييرات
		/// </summary>
		public virtual string LastUpdate
		{
			get;
			set;
		}

		/// <summary>
		/// کد درس
		/// </summary>
		public virtual string LessonCode
		{
			get;
			set;
		}

		/// <summary>
		/// شناسه جدول
		/// </summary>
		public virtual int LessonId
		{
			get;
			set;
		}

		/// <summary>
		/// نام درس
		/// </summary>
		public virtual string LessonName
		{
			get;
			set;
		}

		/// <summary>
		/// نوع درس
		/// </summary>
		public virtual int LessonTypeId
		{
			get;
			set;
		}

		/// <summary>
		/// حداقل نمره قبولي
		/// </summary>
		public virtual int MinPassedGrade
		{
			get;
			set;
		}

		/// <summary>
		/// تعداد ساعت
		/// </summary>
		public virtual short? NumberOfHours
		{
			get;
			set;
		}

		/// <summary>
		/// حداقل نمره قبولی عملی
		/// </summary>
		public virtual decimal PracticalMinGrade
		{
			get;
			set;
		}

		/// <summary>
		/// واحد عملي
		/// </summary>
		public virtual decimal PracticalUnit
		{
			get;
			set;
		}

		/// <summary>
		/// تعداد ساعت عملي
		/// </summary>
		public virtual decimal? PracticeHour
		{
			get;
			set;
		}

		/// <summary>
		/// دروس پيشنياز
		/// </summary>
		public virtual string Prerequisite
		{
			get;
			set;
		}

		/// <summary>
		/// تعداد نيمسال گذرانده پيشنياز
		/// </summary>
		public virtual decimal? PrerequisitePassedTerm
		{
			get;
			set;
		}

		/// <summary>
		/// تعداد واحد گذرانده پيشنياز
		/// </summary>
		public virtual decimal? PrerequisitePassedUnit
		{
			get;
			set;
		}

		/// <summary>
		/// کد گروه آموزشي
		/// </summary>
		public virtual int RegistryGroupId
		{
			get;
			set;
		}

		/// <summary>
		/// کد استاندارد
		/// </summary>
		public virtual string StandardCode
		{
			get;
			set;
		}

		/// <summary>
		/// مقطع درس
		/// </summary>
		public virtual int StudyLevelId
		{
			get;
			set;
		}

		/// <summary>
		/// نوع حق التدريس
		/// </summary>
		public virtual int? TeacherSalaryTypeId
		{
			get;
			set;
		}

		/// <summary>
		/// تعداد ساعت نظري
		/// </summary>
		public virtual decimal? TheoreticalHour
		{
			get;
			set;
		}

		/// <summary>
		/// حداقل نمره قبولی نظری
		/// </summary>
		public virtual decimal TheoreticalMinGrade
		{
			get;
			set;
		}

		/// <summary>
		/// واحد تئوري
		/// </summary>
		public virtual decimal TheoreticalUnit
		{
			get;
			set;
		}

		/// <summary>
		/// واحد
		/// </summary>
		public virtual decimal Unit
		{
			get;
			set;
		}

		/// <summary>
		/// کاربر عامل تغييرات
		/// </summary>
		public virtual int? UpdateByUserId
		{
			get;
			set;
		}



		/// <summary>
		/// Overrides the default equality method.
		/// </summary>
		public override bool Equals(object obj)
		{
				if (ReferenceEquals(this, obj))
						return true;

				return Equals(obj as Lesson);
		}

		/// <summary>
		/// Overrides the default equality method.
		/// </summary>
		public virtual bool Equals(Lesson obj)
		{
				if (obj == null) return false;

				if (Equals(CoPrerequisite, obj.CoPrerequisite) == false) return false;
				if (Equals(ConflictLesson, obj.ConflictLesson) == false) return false;
				if (Equals(DefaultAcceptGradeStateId, obj.DefaultAcceptGradeStateId) == false) return false;
				if (Equals(DefaultRejectGradeStateId, obj.DefaultRejectGradeStateId) == false) return false;
				if (Equals(DefualtLessonStatusId, obj.DefualtLessonStatusId) == false) return false;
				if (Equals(DetailLessonCode, obj.DetailLessonCode) == false) return false;
				if (Equals(EnglishName, obj.EnglishName) == false) return false;
				if (Equals(FacultyId, obj.FacultyId) == false) return false;
				if (Equals(FinancialTypeId, obj.FinancialTypeId) == false) return false;
				if (Equals(GradePassedTypeId, obj.GradePassedTypeId) == false) return false;
				if (Equals(HasActive, obj.HasActive) == false) return false;
				if (Equals(IncompleteGradeTermLimit, obj.IncompleteGradeTermLimit) == false) return false;
				if (Equals(IslamicLessonTypeId, obj.IslamicLessonTypeId) == false) return false;
				if (Equals(LastUpdate, obj.LastUpdate) == false) return false;
				if (Equals(LessonCode, obj.LessonCode) == false) return false;
				if (Equals(LessonId, obj.LessonId) == false) return false;
				if (Equals(LessonName, obj.LessonName) == false) return false;
				if (Equals(LessonTypeId, obj.LessonTypeId) == false) return false;
				if (Equals(MinPassedGrade, obj.MinPassedGrade) == false) return false;
				if (Equals(NumberOfHours, obj.NumberOfHours) == false) return false;
				if (Equals(PracticalMinGrade, obj.PracticalMinGrade) == false) return false;
				if (Equals(PracticalUnit, obj.PracticalUnit) == false) return false;
				if (Equals(PracticeHour, obj.PracticeHour) == false) return false;
				if (Equals(Prerequisite, obj.Prerequisite) == false) return false;
				if (Equals(PrerequisitePassedTerm, obj.PrerequisitePassedTerm) == false) return false;
				if (Equals(PrerequisitePassedUnit, obj.PrerequisitePassedUnit) == false) return false;
				if (Equals(RegistryGroupId, obj.RegistryGroupId) == false) return false;
				if (Equals(StandardCode, obj.StandardCode) == false) return false;
				if (Equals(StudyLevelId, obj.StudyLevelId) == false) return false;
				if (Equals(TeacherSalaryTypeId, obj.TeacherSalaryTypeId) == false) return false;
				if (Equals(TheoreticalHour, obj.TheoreticalHour) == false) return false;
				if (Equals(TheoreticalMinGrade, obj.TheoreticalMinGrade) == false) return false;
				if (Equals(TheoreticalUnit, obj.TheoreticalUnit) == false) return false;
				if (Equals(Unit, obj.Unit) == false) return false;
				if (Equals(UpdateByUserId, obj.UpdateByUserId) == false) return false;
				return true;
		}

		/// <summary>
		/// Generates the hashcode for exact comparing tow instances of the class.
		/// </summary>
		public override int GetHashCode()
		{
				int result = 1;

				result = (result * 397) ^ (CoPrerequisite != null ? CoPrerequisite.GetHashCode() : 0);
				result = (result * 397) ^ (ConflictLesson != null ? ConflictLesson.GetHashCode() : 0);
				result = (result * 397) ^ DefaultAcceptGradeStateId.GetHashCode();
				result = (result * 397) ^ DefaultRejectGradeStateId.GetHashCode();
				result = (result * 397) ^ DefualtLessonStatusId.GetHashCode();
				result = (result * 397) ^ (DetailLessonCode != null ? DetailLessonCode.GetHashCode() : 0);
				result = (result * 397) ^ (EnglishName != null ? EnglishName.GetHashCode() : 0);
				result = (result * 397) ^ FacultyId.GetHashCode();
				result = (result * 397) ^ FinancialTypeId.GetHashCode();
				result = (result * 397) ^ GradePassedTypeId.GetHashCode();
				result = (result * 397) ^ HasActive.GetHashCode();
				result = (result * 397) ^ IncompleteGradeTermLimit.GetHashCode();
				result = (result * 397) ^ IslamicLessonTypeId.GetHashCode();
				result = (result * 397) ^ (LastUpdate != null ? LastUpdate.GetHashCode() : 0);
				result = (result * 397) ^ (LessonCode != null ? LessonCode.GetHashCode() : 0);
				result = (result * 397) ^ LessonId.GetHashCode();
				result = (result * 397) ^ (LessonName != null ? LessonName.GetHashCode() : 0);
				result = (result * 397) ^ LessonTypeId.GetHashCode();
				result = (result * 397) ^ MinPassedGrade.GetHashCode();
				result = (result * 397) ^ NumberOfHours.GetHashCode();
				result = (result * 397) ^ PracticalMinGrade.GetHashCode();
				result = (result * 397) ^ PracticalUnit.GetHashCode();
				result = (result * 397) ^ PracticeHour.GetHashCode();
				result = (result * 397) ^ (Prerequisite != null ? Prerequisite.GetHashCode() : 0);
				result = (result * 397) ^ PrerequisitePassedTerm.GetHashCode();
				result = (result * 397) ^ PrerequisitePassedUnit.GetHashCode();
				result = (result * 397) ^ RegistryGroupId.GetHashCode();
				result = (result * 397) ^ (StandardCode != null ? StandardCode.GetHashCode() : 0);
				result = (result * 397) ^ StudyLevelId.GetHashCode();
				result = (result * 397) ^ TeacherSalaryTypeId.GetHashCode();
				result = (result * 397) ^ TheoreticalHour.GetHashCode();
				result = (result * 397) ^ TheoreticalMinGrade.GetHashCode();
				result = (result * 397) ^ TheoreticalUnit.GetHashCode();
				result = (result * 397) ^ Unit.GetHashCode();
				result = (result * 397) ^ UpdateByUserId.GetHashCode();
				return result;
		}
	}
}