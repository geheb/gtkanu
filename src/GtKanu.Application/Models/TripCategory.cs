namespace GtKanu.Application.Models;

[Flags]
public enum TripCategory
{
    None = 0,
    Junior = 1 << 0,
    JuniorAdvanced = 1 << 1,
    Advanced = 1 << 2,
    YoungPeople = 1 << 3
}
