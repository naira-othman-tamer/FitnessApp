using FCE.Domain.Enums;

namespace FCE.Domain.ValueObject
{
    public record PhysicalStats
    (
    double Weight,
    double Height,
    short Age,
    Gender Gender
    );
}
