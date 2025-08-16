using BaitaHora.Domain.Commons;
using Microsoft.AspNetCore.Mvc;

namespace BaitaHora.Api.Helpers
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result, ControllerBase controller)
        {
            if (result.IsSuccess)
            {
                var code = (int)result.Status;
                var body = ApiResponseHelper.CreateSuccess(result.Title ?? "Sucesso");
                return code == StatusCodes.Status201Created
                    ? controller.StatusCode(code, body)
                    : controller.Ok(body);
            }

            return controller.Problem(
                title: result.Title ?? "Requisição inválida",
                detail: result.Error ?? "Falha",
                statusCode: (int)result.Status
            );
        }

        public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
        {
            if (result.IsSuccess)
            {
                var code = (int)result.Status;
                var body = ApiResponseHelper.CreateSuccess(result.Value!, result.Title ?? "Sucesso");
                return code == StatusCodes.Status201Created
                    ? controller.StatusCode(code, body)
                    : controller.Ok(body);
            }

            return controller.Problem(
                title: result.Title ?? "Requisição inválida",
                detail: result.Error ?? "Falha",
                statusCode: (int)result.Status
            );
        }
    }
}