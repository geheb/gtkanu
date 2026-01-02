using System.ComponentModel.DataAnnotations;

namespace GtKanu.Ui.Annotations;

public sealed class RequiredFieldAttribute : RequiredAttribute
{
    public RequiredFieldAttribute()
    {
        ErrorMessage = "Das Feld '{0}' wird benötigt.";
    }
}
