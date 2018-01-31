using Microsoft.Extensions.Options;
using System;
using StackExchange.Redis;


namespace Helpers.Redis {
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection();
    }

 
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        /// <summary>
        ///     The _connection.
        /// </summary>
        private readonly Lazy<ConnectionMultiplexer> _connection;
        

        private readonly IOptions<ConfigurationOptions> redis;

        public RedisConnectionFactory(IOptions<ConfigurationOptions> redis)
        {
            this._connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect("sl-us-south-1-portal.17.dblayer.com:32543,password=MKAUASDLRNGJOLCP"));
        }

        public ConnectionMultiplexer Connection()
        {
            return this._connection.Value;
        }
    
    }
}