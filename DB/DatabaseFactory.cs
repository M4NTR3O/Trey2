namespace Trey2.DB
{
    public class DatabaseFactory: IDatabaseFactory
    {
        private readonly IConfiguration _configuration;
        public DatabaseFactory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DatabaseClass Create()
        {
            return new DatabaseClass(_configuration);
        }
    }
}
