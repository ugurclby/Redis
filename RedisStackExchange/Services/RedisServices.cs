using StackExchange.Redis;

namespace RedisStackExchange.Services
{
    public class RedisServices
    {
        private readonly string _redisHost;
        private readonly string _redisPort;
        
        private IDatabase _database { get; set; } 

        private ConnectionMultiplexer _ConnectionMultiplexer;

        public RedisServices(IConfiguration configuration )
        {
            _redisHost = configuration["Redis:Host"];
            _redisPort = configuration["Redis:Port"];   
        }
        public void Connect()
        {  
            var config = $"{_redisHost}:{_redisPort}";
            _ConnectionMultiplexer = ConnectionMultiplexer.Connect(config); 
        }
        public IDatabase GetDb(int dbIndex) => _database = _ConnectionMultiplexer.GetDatabase(dbIndex); 
    }
}
