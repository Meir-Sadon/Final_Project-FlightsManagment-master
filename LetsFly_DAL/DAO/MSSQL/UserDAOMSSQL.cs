﻿using System; using System.Collections.Generic; using System.Data; using System.Data.SqlClient; using System.Linq; using System.Text; using System.Threading.Tasks;  namespace LetsFly_DAL {     internal class UserDAOMSSQL : IUserDAO     {         // Add New UserName To Data Base.         public void AddUserName(User user, out long userId)         {             userId = 0;             using (SqlConnection conn = new SqlConnection(FlyingCenterConfig.CONNECTION_STRING))             {                 using (SqlCommand cmd = new SqlCommand($"Add_User", conn))                 {                     cmd.Connection.Open();                      cmd.Parameters.AddWithValue("@User_Name", user.UserName);                     cmd.Parameters.AddWithValue("@Password", user.Password);                     cmd.Parameters.AddWithValue("@Type", user.Type);                     cmd.Parameters.AddWithValue("@Is_Verified", user.IsVerified);                     cmd.CommandType = CommandType.StoredProcedure;                     userId = (long)cmd.ExecuteScalar();                 }             }         }          #region Verify New Customer Account.         /// <summary>         /// Function To Verify New Customer Account.         /// </summary>         /// <param name="email"></param>         /// <returns>bool</returns>         public bool VerifyNewCustomerEmail(string email)         {             using (SqlConnection conn = new SqlConnection(FlyingCenterConfig.CONNECTION_STRING))             {                 using (SqlCommand cmd = new SqlCommand($"VerifyNewCustomerEmail", conn))                 {                     cmd.Connection.Open();                      cmd.Parameters.AddWithValue("@email", email);                     cmd.CommandType = CommandType.StoredProcedure;                     if (cmd.ExecuteNonQuery() > 0)                         return true;                     return false;                 }             }         }         #endregion          // Remove Some UserName From The Data Base.         public void RemoveUserName(User user)         {             using (SqlConnection conn = new SqlConnection(FlyingCenterConfig.CONNECTION_STRING))             {                 conn.Open();                 using (SqlCommand cmd = new SqlCommand($"Delete From Users Where User_Name = '{user.UserName}'", conn))                 {                     cmd.ExecuteNonQuery();                 }             }         }          // Change UserName For Some User.         public void UpdateUserName(string oldUserName, string newUserName)         {             using (SqlConnection conn = new SqlConnection(FlyingCenterConfig.CONNECTION_STRING))             {                 conn.Open();                 using (SqlCommand cmd = new SqlCommand($"Update Users Set User_Name = '{newUserName}' Where User_Name = '{oldUserName}'", conn))                 {                     cmd.ExecuteNonQuery();                 }             }         }          // Change Password Of Current Administrator.         public bool TryChangePasswordForUser(User user, string oldPassword, string newPassword)         {             using (SqlConnection conn = new SqlConnection(FlyingCenterConfig.CONNECTION_STRING))             {                 conn.Open();                 using (SqlCommand cmd = new SqlCommand($"Update Users Set Password = '{newPassword}' Where User_Name = '{user.UserName}' And Password = '{oldPassword}'", conn))                 {                     using (SqlDataReader reader = cmd.ExecuteReader())                     {                         if (reader.RecordsAffected > 0)                             return true;                     }                 }             }             return false;         }          //Force Change Password For Airline/Customer From Admin.         public void ForceChangePasswordForUser(User user, string newPassword)         {             using (SqlConnection conn = new SqlConnection(FlyingCenterConfig.CONNECTION_STRING))             {                 conn.Open();                 using (SqlCommand cmd = new SqlCommand($"Update Users Set Password = '{newPassword}' Where User_Name = '{user.UserName}'", conn))                 {                     using (SqlDataReader reader = cmd.ExecuteReader())                     {                         if (reader.RecordsAffected > 0)                             return;                     }                 }             }             throw new UserNotExistException("Sorry, But We Don't Found This User.");         }          // Get User By UserName.         public User GetUserByUserName(string userName)         {             User user = null;             if (userName.ToUpper() == FlyingCenterConfig.ADMIN_NAME)                 user = new User(-1, FlyingCenterConfig.ADMIN_NAME, FlyingCenterConfig.ADMIN_PASSWORD, UserTypes.Administrator, true) ;             else             {                 using (SqlConnection conn = new SqlConnection(FlyingCenterConfig.CONNECTION_STRING))                 {                      conn.Open();                     using (SqlCommand cmd = new SqlCommand($"Select * from Users where User_Name = '{userName}'", conn))                     {                         using (SqlDataReader reader = cmd.ExecuteReader())                         {                             if (reader.Read() == true && user == null)                             {                                  UserTypes.TryParse((string)reader["Type"], out UserTypes theType);                                 user = new User((long)reader["Id"], (string)reader["User_Name"], (string)reader["Password"], theType, (bool)reader["Is_Verified"]);                             }                         }                     }                 }             }             return user;         }          // Get User By Id.         public User GetUserById(long id)         {             User user = null;             {                 using (SqlConnection conn = new SqlConnection(FlyingCenterConfig.CONNECTION_STRING))                 {                     conn.Open();                     using (SqlCommand cmd = new SqlCommand($"Select * from Users where Id = {id}", conn))                     {                         using (SqlDataReader reader = cmd.ExecuteReader())                         {                             if (reader.Read() == true)                             {                                 UserTypes.TryParse((string)reader["Type"], out UserTypes theType);                                 user = new User((long)reader["Id"], (string)reader["User_Name"], (string)reader["Password"], theType, (bool)reader["Is_Verified"]);                             }                         }                     }                 }             }             return user;         }     } } 