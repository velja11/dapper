using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using DotnetAPI.Data;
using System.Linq.Expressions;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{

    DataContextDapper _dapper;

    public UserController(IConfiguration config){
          _dapper = new DataContextDapper(config);
    }

   [HttpGet("TestConnection")]
   public DateTime TestConnection(){
       return _dapper.LoadSingleData<DateTime>("SELECT GETDATE()");
   } 
   
   [HttpGet("test/{testValue}")]
   public string[] Test(string testValue){

        string[] apiRes = new string[] {"test1", "test2", testValue};

        return apiRes;
   }


   [HttpGet("GetUsers")]
    public IEnumerable<User> GetUsers(){
        IEnumerable<User> users;

        string sql = @"
        SELECT [UserId]
      ,[FirstName]
      ,[LastName]
      ,[Email]
      ,[Gender]
      ,[Active]
        FROM [DotNetCourseDatabase].[TutorialAppSchema].[Users]
    ";

        users = _dapper.LoadData<User>(sql);

        return users;



    }

    [HttpGet("GetUsers/{userId}")]
    public User GetSingleUser(int userId){
       

        string sql = @"
                      SELECT [UserId]
                    ,[FirstName]
                    ,[LastName]
                    ,[Email]
                    ,[Gender]
                    ,[Active]
                    FROM [DotNetCourseDatabase].[TutorialAppSchema].[Users]
                    WHERE UserId = " +userId.ToString();

        User user = _dapper.LoadSingleData<User>(sql);

        return user;

    }

    [HttpPut("PutUsers")]
    public IActionResult EditUser(User user){

        string sql = @" 
            UPDATE TutorialAppSchema.Users
            SET FirstName = '" + user.FirstName + 
            "', LastName = '" + user.LastName +
            "', Email = '" + user.Email +
            "', Gender = '" + user.Gender +
            "', Active = '" + user.Active +
            "' WHERE UserId = " + user.UserId;

         if(_dapper.ExecuteSQL(sql)){
            return Ok();
         }   

         throw new Exception("Error");
    }


    [HttpPost("PostUser")]
    public IActionResult PostUser(UserDto user){

        string sql = @" INSERT INTO TutorialAppSchema.Users 
                        (FirstName, 
                        LastName,
                        Email,
                        Gender,
                        Active) 
                        VALUES 
                       ('"+user.FirstName+"', '"+user.LastName+"','"+user.Email+"', '"+user.Gender+"', '"+user.Active+"')";


        if(_dapper.ExecuteSQL(sql)){
            return Ok();
        }


        throw new Exception("Error");
    }

    [HttpDelete("DeleteUser/{userID}")]
    public IActionResult DeleteUser(int userID){

        string sql = @"DELETE
                    FROM TutorialAppSchema.Users
                    WHERE UserId = " +userID;

        if(_dapper.ExecuteSQL(sql)){
            return Ok();
        }


        throw new Exception("Error");
    }


    [HttpGet("GetJobsInfo")]
    public IEnumerable<UserJobInfo> GetJobsInfo(){
            string sql = @"
                SELECT UserId, JobTitle, Department
                FROM DotNetCourseDatabase.TutorialAppSchema.UserJobInfo";

                IEnumerable<UserJobInfo> jobList;

               jobList = _dapper.LoadData<UserJobInfo>(sql);

               return jobList;

    }

    [HttpGet("GetSingleJobInfo/{userId}")]
    public UserJobInfo GetSingleJobInfo(int userId){
        string sql = @"
                SELECT UserId, JobTitle, Department
                FROM DotNetCourseDatabase.TutorialAppSchema.UserJobInfo
                WHERE UserId = " + userId;
        
        
         UserJobInfo userInfo =  _dapper.LoadSingleData<UserJobInfo>(sql);


        if(userInfo != null){
            return userInfo;
        }

        throw new Exception("Error");

        
    }

    [HttpPut("PutUserJobInfo")]
    public IActionResult PutUserJobInfo(UserJobInfo userInfo){
        string sql = @"
            UPDATE DotNetCourseDatabase.TutorialAppSchema.UserJobInfo
            SET JobTitle = '" + userInfo.JobTitle + 
            "', Department = '" + userInfo.Department + 
            "' WHERE UserId = " + userInfo.UserId.ToString();

        if(_dapper.ExecuteSQL(sql)){
            return Ok();
        }

        throw new Exception("Error");
             
        
    }


    [HttpPost("PostUserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo userInfo){
        
        string sql = @"
            INSERT INTO DotNetCourseDatabase.TutorialAppSchema.UserJobInfo
            (UserId, JobTitle, Department)
            VALUES('"+userInfo.UserId+"','"+userInfo.JobTitle+"','"+userInfo.Department+"')";

        if(_dapper.ExecuteSQLWithRows(sql) > 0){
            return Ok();
        }

        throw new Exception("Error");

    }

    [HttpDelete("DeleteJobInfo/{userId}")]
    public IActionResult DeleteJobInfo(int userId){

        string sql = @"
        DELETE FROM DotNetCourseDatabase.TutorialAppSchema.UserJobInfo
        WHERE UserId = " + userId.ToString();

        if(_dapper.ExecuteSQLWithRows(sql) > 0){
            return Ok();
        }

        throw new Exception("Error");
    }
    
}

