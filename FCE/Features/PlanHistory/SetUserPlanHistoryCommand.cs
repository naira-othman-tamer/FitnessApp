using FCE.Features.Common.Helpers;

namespace FCE.Features.PlanHistory;

public record SetUserPlanHistoryCommand(Guid UserId) : ICommandRequest<bool>;

