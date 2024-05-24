using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Data
{


     class DataContextDapper{

        private readonly IConfiguration _config;


        public DataContextDapper(IConfiguration config){
                _config = config;
        }

        public IEnumerable<T> LoadData<T>(string sql){
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Query<T>(sql);
        }

        public T LoadSingleData<T>(string sql){
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.QuerySingle<T>(sql);
        }

        public bool ExecuteSQL(string sql){
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql) > 0;
        }

        public int ExecuteSQLWithRows(string sql){
            IDbConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            return dbConnection.Execute(sql);
        }

        public bool ExecuteWithParams(string sql, List<SqlParameter> parameters){

            SqlCommand command = new SqlCommand(sql);

            foreach(SqlParameter param in parameters){
                command.Parameters.Add(param);
            }

            SqlConnection dbConnection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));

            dbConnection.Open();

            command.Connection = dbConnection;

            int rowsAffected = command.ExecuteNonQuery();

            dbConnection.Close();

            return rowsAffected > 0;
        }
    }
}