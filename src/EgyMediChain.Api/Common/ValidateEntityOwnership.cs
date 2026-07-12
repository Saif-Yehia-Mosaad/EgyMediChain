using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EgyMediChain.Api.Common;

// Put this on a controller (or single action) that has an int route parameter like
// {factoryId}/{warehouseId}/{pharmacyId}. It makes sure a FactoryUser/WarehouseUser/PharmacyUser
// can only ever request data for the entity that's actually stored in THEIR OWN token -
// no matter what id they put in the URL. Ministry roles (SuperAdmin/MinistryAdmin/MinistryViewer)
// are exempt, since the Ministry is allowed to look at any entity.
//
// Usage: put [ValidateEntityOwnership("factoryId")] on FactoryDashboardController,
// [ValidateEntityOwnership("warehouseId")] on WarehouseDashboardController,
// [ValidateEntityOwnership("pharmacyId")] on PharmacyDashboardController.
public class ValidateEntityOwnershipAttribute : Attribute, IActionFilter
{
    private readonly string _routeKey;

    private static readonly string[] MinistryRoles =
    {
        "SuperAdmin", "MinistryAdmin", "MinistryViewer"
    };

    public ValidateEntityOwnershipAttribute(string routeKey)
    {
        _routeKey = routeKey;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;

        if (role != null && MinistryRoles.Contains(role))
            return; // Ministry roles can look at any entity

        if (!context.ActionArguments.TryGetValue(_routeKey, out var routeValue) || routeValue is not int routeEntityId)
            return; // this action doesn't have that route parameter - nothing to check

        var entityIdClaim = user.FindFirst("entityId")?.Value;

        if (!int.TryParse(entityIdClaim, out var tokenEntityId) || tokenEntityId != routeEntityId)
        {
            context.Result = new ObjectResult(new { message = "You don't have access to this entity's data." })
            {
                StatusCode = 403
            };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // nothing to do after the action runs
    }
}
