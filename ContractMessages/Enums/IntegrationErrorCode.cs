using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ContractMessages.Enums
{
    public enum IntegrationErrorCode
    {
        None = 0,
        NoMatchingPlanFound,
        InvalidRequest,
        InternalError
    }
}
