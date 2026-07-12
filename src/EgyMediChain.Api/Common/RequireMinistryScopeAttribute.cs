using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EgyMediChain.Api.Common;

// For Ministry-tier controllers (Factories/Warehouses/Pharmacies/etc). A MinistryAdmin or
// MinistryViewer account can optionally be scoped to just ONE entity type (Factory, Warehouse,
// or Pharmacy) - stored in the SAME "entityType" JWT claim used for FactoryUser/WarehouseUser/
// PharmacyUser accounts (see AdminController.AddMinistryAdmin). If that claim is empty or
// literally "Ministry", the account is unscoped and can see everything, same as before.
//
// SuperAdmin always bypasses this check completely, regardless of what's in the claim.
//
// Usage: [RequireMinistryScope("Factory")] on FactoriesController,
// [RequireMinistryScope("Warehouse")] on WarehousesController,
// [RequireMinistryScope("Pharmacy")] on PharmaciesController.
public class RequireMinistryScopeAttribute : Attribute, IActionFilter
{
    private readonly string _requiredScope;

    public RequireMinistryScopeAttribute(string requiredScope)
    {
        _requiredScope = requiredScope;
    }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var user = context.HttpContext.User;
        var role = user.FindFirst(ClaimTypes.Role)?.Value;

        if (role == "SuperAdmin")
            return; // SuperAdmin always sees everything

        // Not a Ministry role at all (shouldn't normally happen here since [Authorize(Roles=...)]
        // already restricts these controllers to Ministry roles) - let the [Authorize] attribute
        // handle rejecting it, don't duplicate that logic here.
        if (role != "MinistryAdmin" && role != "MinistryViewer")
            return;

        var entityScope = user.FindFirst("entityType")?.Value;

        if (string.IsNullOrEmpty(entityScope) || entityScope == "Ministry")
            return; // unscoped Ministry account - sees everything

        if (!string.Equals(entityScope, _requiredScope, StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new ObjectResult(new
            {
                message = $"This account is scoped to {entityScope} only and can't access {_requiredScope} data."
            })
            { StatusCode = 403 };
        }
    }

    public void OnActionExecuted(ActionExecutedContext context) { }
}
