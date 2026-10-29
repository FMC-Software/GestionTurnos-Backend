using GestionTurnos.Application.Request;
using GestionTurnos.Application.Response;
using GestionTurnos.Domain.Entities;

namespace GestionTurnos.Application.Mapper
{
    public static class BugReportMapper
    {
        public static BugReport ToEntity(this BugReportCreateRequest request, Guid? businessId, Guid? userId, string? userName)
        {
            return new BugReport
            {
                Type = request.Type,
                Description = request.Description,
                StepsToReproduce = request.StepsToReproduce,
                Browser = request.Browser,
                PageUrl = request.PageUrl,
                BusinessId = businessId,
                UserId = userId,
                UserName = userName
            };
        }

        // El negocio es opcional: si quien reporto fue un SysAdmin (sin negocio
        // asociado) o el negocio ya no existe, BusinessId puede no resolver a
        // nada y los campos de negocio quedan null.
        public static BugReportResponse ToResponse(this BugReport bugReport, Business? business = null)
        {
            return new BugReportResponse
            {
                Id = bugReport.Id,
                CreatedAt = bugReport.CreatedAt,
                Type = bugReport.Type,
                Description = bugReport.Description,
                StepsToReproduce = bugReport.StepsToReproduce,
                Browser = bugReport.Browser,
                PageUrl = bugReport.PageUrl,
                BusinessId = bugReport.BusinessId,
                BusinessName = business?.Name,
                BusinessUrl = business?.Url,
                BusinessCategory = business?.TypeBusiness,
                BusinessLogoUrl = business?.UrlLogo,
                UserId = bugReport.UserId,
                UserName = bugReport.UserName,
                Status = bugReport.Status
            };
        }
    }
}
