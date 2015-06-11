namespace Genitor.Library.Core.Mapping
{
	public interface IMapper<in TFtom, out TTo>
	{
		TTo Map(TFtom from);
	}
}
