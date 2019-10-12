﻿namespace NetRpc
{
    public interface IClientProxyFactory
    {
        IClientProxy<TService> CreateProxy<TService>(string name);
    }
}