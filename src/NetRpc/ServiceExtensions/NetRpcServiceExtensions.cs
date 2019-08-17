﻿using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace NetRpc
{
    public static class NetRpcServiceExtensions
    {
        public static IServiceCollection AddNetRpcService(this IServiceCollection services, Action<MiddlewareOptions> middlewareConfigureOptions = null)
        {
            if (middlewareConfigureOptions != null)
                services.Configure(middlewareConfigureOptions);
            services.AddSingleton<MiddlewareBuilder>();
            return services;
        }

        public static IServiceCollection AddNetRpcServiceContract(this IServiceCollection services, Type implementationType,
            ContractLifeTime contractLifeTime = ContractLifeTime.Singleton)
        {
            services.Configure<ContractOptions>(i => i.Contracts.Add(implementationType));
            switch (contractLifeTime)
            {
                case ContractLifeTime.Singleton:
                    services.AddSingleton(implementationType);
                    break;
                case ContractLifeTime.Scoped:
                    services.AddScoped(implementationType);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(contractLifeTime), contractLifeTime, null);
            }

            return services;
        }

        public static IServiceCollection AddNetRpcServiceContract(this IServiceCollection services, IEnumerable<Type> implementationTypes,
            ContractLifeTime contractLifeTime = ContractLifeTime.Singleton)
        {
            foreach (var t in implementationTypes)
                services.AddNetRpcServiceContract(t, contractLifeTime);
            return services;
        }

        public static IServiceCollection AddNetRpcServiceContract<TImplementationType>(this IServiceCollection services,
            ContractLifeTime contractLifeTime = ContractLifeTime.Singleton) where TImplementationType : class
        {
            return services.AddNetRpcServiceContract(typeof(TImplementationType), contractLifeTime);
        }

        public static IServiceCollection AddNetRpcClient<TClientConnectionFactoryImplementation, TService>(this IServiceCollection services,
            Action<NetRpcClientOption> configureOptions)
            where TClientConnectionFactoryImplementation : class, IConnectionFactory
        {
            if (configureOptions != null)
                services.Configure(configureOptions);
            services.AddSingleton<IConnectionFactory, TClientConnectionFactoryImplementation>();
            services.AddSingleton<ClientProxy<TService>>();
            return services;
        }

        public static object[] GetContractInstances(this IServiceProvider serviceProvider, ContractOptions options)
        {
            return options.Contracts.ConvertAll(serviceProvider.GetRequiredService).ToArray();
        }
    }
}