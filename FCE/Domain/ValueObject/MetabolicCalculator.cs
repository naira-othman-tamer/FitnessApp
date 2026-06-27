using FCE.Domain.Enums;

namespace FCE.Domain.ValueObject
{
    public record MetabolicCalculator
    (
    double BMR,
    double TDEE,
    double CalorieTarget,
    CalorieTarget Tier
    );
}
