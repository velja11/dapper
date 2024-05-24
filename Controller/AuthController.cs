using System.Security.Cryptography;
using System.Text;
using AutoMapper.Internal.Mappers;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;


namespace DotnetAPI.Controllers{


public class AuthController:ControllerBase
{
    private readonly DataContextDapper _dapper;

    private readonly IConfiguration _config;

    public AuthController(IConfiguration config){
          _dapper = new DataContextDapper(config);
          _config = config;
    }


    [HttpPost("Register")]
    public IActionResult Register(UserForRegistrationDto userRegistration){

        if (userRegistration.Password == userRegistration.PasswordConfirm){

            string sql = @"Select Email from [DotNetCourseDatabase].[TutorialAppSchema].[Auth] where Email = '"+userRegistration.Email+"'";

            IEnumerable<string> existingEmail = _dapper.LoadData<string>(sql);

            if(existingEmail.Count() == 0 ){

                byte [] passwordSalt = new byte[128/8];
                using(RandomNumberGenerator rng = RandomNumberGenerator.Create()){
                    rng.GetNonZeroBytes(passwordSalt);
                }

               byte [] passwordHash =  GetPasswordHash(userRegistration.Password, passwordSalt);

                string sqlAddToAuth = @"
                INSERT INTO [DotNetCourseDatabase].[TutorialAppSchema].[Auth]
                VALUES('"+userRegistration.Email+"', @PasswordHash, @PasswordSalt)";

                List<SqlParameter> sqlParameters = new List<SqlParameter>();

                SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", System.Data.SqlDbType.VarBinary);
                passwordHashParameter.Value = passwordHash;

                SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", System.Data.SqlDbType.VarBinary);
                passwordSaltParameter.Value = passwordSalt;

                sqlParameters.Add(passwordHashParameter);
                sqlParameters.Add(passwordSaltParameter);

                if(_dapper.ExecuteWithParams(sqlAddToAuth, sqlParameters)){

                    return Ok();
                }

                throw new Exception("Failed to register");

                
            }

            throw new Exception("User already exists!");

            
        }

        throw new Exception("Password is not ok");
        
    }

    [HttpPost("Login")]
    public IActionResult Login(UserForLoginDto userLogin){

        string sqlLogin = @"
        SELECT [PasswordHash], [PasswordSalt] from [DotNetCourseDatabase].[TutorialAppSchema].[Auth] where Email = '"+userLogin.Email+"'";

        UserForLoginConfirmationDto userForConfirmation = _dapper.LoadSingleData<UserForLoginConfirmationDto>(sqlLogin);


        byte [] passwordHash = GetPasswordHash(userLogin.Password, userForConfirmation.PasswordSalt);

        for(int i = 0; i < passwordHash.Length; i++){

            if(passwordHash[i] != userForConfirmation.PasswordHash[i]){
                return StatusCode(401,"Incorret password");
            }
        }


        return Ok();
    }

    public byte[] GetPasswordHash(string password, byte[] passwordSalt){

        string passwordSaltPlusString = _config.GetSection("AppSettings:Passwordkey").Value + Convert.ToBase64String(passwordSalt);

         return KeyDerivation.Pbkdf2(
                    password: password,
                    salt: Encoding.ASCII.GetBytes(passwordSaltPlusString),
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8
                );
    }

}

}