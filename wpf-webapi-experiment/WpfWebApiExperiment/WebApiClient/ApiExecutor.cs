﻿using System.Threading.Tasks;
using Ninject;
using WpfWebApiExperiment.Services;

namespace WpfWebApiExperiment.WebApiClient
{
    public class ApiExecutor : IApiExecutor
    {
        private readonly ILongOperationExecutor _longOperationExecutor;
        private readonly IApiClient _apiClient;

        [Inject]
        public ApiExecutor(ILongOperationExecutor longOperationExecutor, IApiClient apiClient)
        {
            _longOperationExecutor = longOperationExecutor;
            _apiClient = apiClient;
        }

        public async Task<TResponse> Execute<TResponse>(IApiRequest<TResponse> request)
            where TResponse : new()
        {
            return await _longOperationExecutor.Execute(() =>
            {
                var result = _apiClient.Execute(request);
                return result;
            });
        }
    }
}
