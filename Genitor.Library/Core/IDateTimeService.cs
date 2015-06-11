namespace Genitor.Library.Core
{
	using System;

	public interface IDateTimeService
	{
		DateTime UtcNow { get; set; }
	}
}