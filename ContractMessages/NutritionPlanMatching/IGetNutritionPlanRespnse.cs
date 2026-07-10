using ContractMessages.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractMessages.NutritionPlanMatching
{
    public interface IGetNutritionPlanResponse
    {
        Guid planId { get; }
        string NutritionPlanName { get; }
        IntegrationErrorCode ErrorCode { get; }
        bool IsSuccess { get; }
    }
}
