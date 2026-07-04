using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NutritionService.Common.StandardizedResponse
{
    public enum StatusCode
    {
        // success
        Success = 200,
        Created = 201,
        NoContent = 204,

        // process
        Redirect = 303,

        // auth
        BadRequest = 400,

        Unauthorized = 401,
        Forbidden = 403,
        NotFound = 404,
        MissingProfiles = 405,
        Conflict = 409,
        Gone = 410,
        ValidationError = 422,
        Locked = 423,
        TooManyRequests = 429,

        // errors
        InternalServerError = 500,

        NotImplemented = 501,
        ServiceUnavailable = 503,
        GeneralError = 550,
        DataCorruption = 590,
    }
}
