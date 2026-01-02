using GtKanu.Core.Converter;

namespace GtKanu.Core.Entities;

public interface IDtoMapper<TModel> 
    where TModel : struct, IDto
{
    TModel ToDto(GermanDateTimeConverter dc);
    void FromDto(TModel model);
}
