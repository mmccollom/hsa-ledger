using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace HsaLedger.Server;

public class RoleDocumentFilter : IDocumentFilter
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public RoleDocumentFilter(IHttpContextAccessor httpContextAccessor) =>
        _httpContextAccessor = httpContextAccessor;

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        if (_httpContextAccessor.HttpContext == null)
        {
            return;
        }
        
        // we fetch role from query string.
        var query = _httpContextAccessor.HttpContext.Request.QueryString.ToString();
        string? role = HttpUtility.ParseQueryString(query).Get("role");

        // if role is empty we show all api's else we need to filter by role 
        if (string.IsNullOrEmpty(role))
            return;

        // create a list of path which needs to be excluded.
        List<string> pathToRemove = new();
        foreach (var item in context.ApiDescriptions)
        {
            var rolesFromAttribute = item.CustomAttributes()
                .OfType<AuthorizeAttribute>()
                .Select(a => a.Roles)
                .Distinct().ToList();

            if (rolesFromAttribute.Any())
            {
                string? roleAttribute = rolesFromAttribute.FirstOrDefault();
                if (roleAttribute != null)
                {
                    string[] roles = roleAttribute.Split(',');
                    // if roles contains the given role then it mean i have to keep this path
                    if (!roles.Contains(role))
                    {
                        pathToRemove.Add("/" + item.RelativePath);
                    }
                }
            }
        }

        // finally remove not required paths
        pathToRemove.ForEach(x => { swaggerDoc.Paths.Remove(x); });
    }
}