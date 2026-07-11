using ContractMessages.Enums;
using ContractMessages.NutritionPlanMatching;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NutritionService.Data;
using NutritionService.Domain.Entities;
using Repository.Layer.Interfaces;

namespace NutritionService.Integrations.Consumers;

public sealed class NutritionPlanMatchingConsumer(IUnitOfWork<NutritionDbContext> unitOfWork)
    : IConsumer<IGetNutritionPlanRequest>
{
    public async Task Consume(ConsumeContext<IGetNutritionPlanRequest> context)
    {
        var calorieTarget = context.Message.CalorieTarget;

        if (calorieTarget <= 0)
        {
            await RespondFailure(context, IntegrationErrorCode.InvalidRequest);
            return;
        }

        var plan = await unitOfWork.Repository<MealPlan, Guid>().Query()
            .Where(x => x.TargetCalorieRangeMin <= calorieTarget && x.TargetCalorieRangeMax >= calorieTarget)
            .OrderBy(x => x.TargetCalorieRangeMin)
            .Select(x => new
            {
                x.Id,
                x.Name
            })
            .FirstOrDefaultAsync(context.CancellationToken);

        if (plan is null)
        {
            await RespondFailure(context, IntegrationErrorCode.NoMatchingNutritionPlanFound);
            return;
        }

        await context.RespondAsync<IGetNutritionPlanResponse>(new
        {
            IsSuccess = true,
            NutritionPlanId = plan.Id,
            NutritionPlanName = plan.Name,
            ErrorCode = IntegrationErrorCode.None
        });
    }

    private static Task RespondFailure(ConsumeContext<IGetNutritionPlanRequest> context, IntegrationErrorCode errorCode) =>
        context.RespondAsync<IGetNutritionPlanResponse>(new
        {
            IsSuccess = false,
            NutritionPlanId = Guid.Empty,
            NutritionPlanName = string.Empty,
            ErrorCode = errorCode
        });
}
