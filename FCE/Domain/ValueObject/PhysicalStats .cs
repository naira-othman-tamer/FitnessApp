using FCE.Domain.Enums;

namespace FCE.Domain.ValueObject
{
    public record PhysicalStats
    (
    double Weight, // 40-200 kg
    double Height, // 140-220 cm
    short Age, // 16-100 years
    Gender Gender
    );
}
