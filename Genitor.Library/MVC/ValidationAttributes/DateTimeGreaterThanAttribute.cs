namespace Genitor.Library.MVC.ValidationAttributes
{
	using System;
	using System.ComponentModel.DataAnnotations;

	//TODO: nahradit univerzalnejsim
	public class DateTimeGreaterThanAttribute : ValidationAttribute
	{
		public int DayIntervalGreaterThanNow { get; set; }

		public override bool IsValid(object value)
		{
			var dateStart = (DateTime?)value;
			return !dateStart.HasValue || dateStart.Value > DateTime.Now.AddDays(DayIntervalGreaterThanNow);
		}
	}
}