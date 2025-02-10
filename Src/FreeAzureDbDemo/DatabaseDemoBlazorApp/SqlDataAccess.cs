using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace DatabaseDemoBlazorApp;

public class SqlDataAccess
{
  private readonly IConfiguration _config;

  public SqlDataAccess(IConfiguration config)
  {
    this._config = config;
  }

  public async Task<List<T>> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName = "default")
  {
    using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionStringName));
    var data = await connection.QueryAsync<T>(storedProcedure, parameters, commandType: CommandType.StoredProcedure);
    return data.ToList() as List<T>;
  }
}