using System.Data;

namespace Modules.UserAcess.Application.Data;

public interface ISqlConnectionFactory
{
    IDbConnection GetOpenConnection();
    IDbConnection CreateNewConnection();
    string GetConnectionString();
}