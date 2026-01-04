using GtKanu.Application.Converter;
using GtKanu.Application.Models;

namespace GtKanu.Infrastructure.Database.Entities;

public interface IDtoMapper<TModel> 
    where TModel : struct, IDto
{
    TModel ToDto(GermanDateTimeConverter dc);
    void FromDto(TModel model);
}
