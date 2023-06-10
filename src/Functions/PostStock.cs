using azure_function_example_csharp.ValidationHelpers;
using Function.Domain.Helpers;
using Function.Domain.Models;
using Function.Domain.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Example.Function
{
    public class PostStock
    {
        private readonly IStockDataProvider _stockDataProvider;
        private readonly IHttpHelper _httpHelper;
        private readonly ILogger<GetOpenStockPriceForSymbol> _logger;

        public PostStock(ILogger<GetOpenStockPriceForSymbol> logger)
        {
            _logger = logger;
        }

        [Function("PostStock")]
        [OpenApiOperation(operationId: "PostStock")]
        [OpenApiParameter(name: "symbol", In = ParameterLocation.Path, Required = true, Type = typeof(List<Stock>), Description = "Symbol to get stock data from")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(Stock), Description = "OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Anonymous,
                "post",
                Route = null
            )] HttpRequestData req)
        {
            _logger.LogInformation($"Posting stock price for symbol: {req}");

            List<Stock> stocks = null;
            using (var stream = new StreamReader(req.Body))
            {
                string requestBody = await stream.ReadToEndAsync();

                stocks = JsonConvert.DeserializeObject<List<Stock>>(requestBody);
            }

            bool isListValid = true;

            Dictionary<string, IEnumerable<string>> errorStocks = new Dictionary<string, IEnumerable<string>>();

            foreach (var stock in stocks)
            {
                ValidationWrapper<Stock> validationWrapper = ValidationWrapperFactory.Create(stock);

                bool isValid = validationWrapper.IsValid;

                if (!isValid)
                {
                    var validationErrors = validationWrapper.ValidationResults.Select(x => x.ErrorMessage);
                    errorStocks.Add(JsonConvert.SerializeObject(stock), validationErrors);
                    isListValid = false;
                }
            }

            if (!isListValid)
            {
                return new BadRequestObjectResult(errorStocks);
            }

            return new OkResult();
        }
    }
}
