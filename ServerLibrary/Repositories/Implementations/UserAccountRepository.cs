using BaseLibrary.DTOs;
using BaseLibrary.Entities;
using BaseLibrary.Responses;
using BaseLibrary.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ServerLibrary.Data;
using ServerLibrary.Helpers;
using ServerLibrary.Repositories.Contracts;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ServerLibrary.Repositories.Implementations
{
    public class UserAccountRepository(IOptions<JwtSection> config, AppDbContext app) : IUserAccount
    {
        public async Task<GeneralResponse> CreateAsync(Register user)
        {
            if (user is null) return new GeneralResponse(false, "Model is empty");
            var chkUsr = await FindUserByEmail(user.Email!);
            if (chkUsr != null) return new GeneralResponse(false, "User registered already");
            // save user
            var applicationUser = await AddToDatabase(new ApplicationUser
            {
                Fullname = user.Fullname,
                Email = user.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(user.Password)
            });

            // check, create and assign role
            var chkAdminRole = await app.SystemRoles.FirstOrDefaultAsync(x => x.Name!.Equals(Constants.Admin));
            if(chkAdminRole is null)
            {
                var createAdminRole = await AddToDatabase(new SystemRole() { Name = Constants.Admin });
                await AddToDatabase(new UserRole() { RoleId = createAdminRole.Id, UserId = applicationUser.Id });
                return new GeneralResponse(true, "Account created");
            }

            var chkUserRole = await app.SystemRoles.FirstOrDefaultAsync(x => x.Name!.Equals(Constants.User));
            SystemRole response = new();
            if (chkUserRole is null)
            {
                response = await AddToDatabase(new SystemRole() { Name = Constants.User });
                chkUserRole = response;
                //await AddToDatabase(new UserRole() { RoleId = response.Id, UserId = applicationUser.Id });
            }
            await AddToDatabase(new UserRole() { RoleId = chkUserRole.Id, UserId = applicationUser.Id });
            return new GeneralResponse(true, "Account created");
        }

        public async Task<LoginResponse> SignInAsync(Login user)
        {
            if (user is null) return new LoginResponse(false, "Model is empty");
            var applicationUser = await FindUserByEmail(user.Email!);
            if (applicationUser is null) return new LoginResponse(false, "User not found");

            // verify password
            if(!BCrypt.Net.BCrypt.Verify(user.Password, applicationUser.Password))
            {
                return new LoginResponse(false, "Password is invalid");
            }
            var getUserRole = await FindUserRole(applicationUser.Id);
            if (getUserRole is null) return new LoginResponse(false, "User role is not found");

            var getRoleName = await FindRoleName(getUserRole.RoleId);
            if (getRoleName is null) return new LoginResponse(false, "Rolename is not found");

            string jwt = GenerateToken(applicationUser, getRoleName!.Name!);
            string refreshToken = GenerateRefreshToken();

            // Save and refresh token to the database
            var findToken = await app.RefreshTokenInfos.FirstOrDefaultAsync(x => x.UserId == applicationUser.Id);
            if(findToken is not null)
            {
                findToken!.Token = refreshToken;
                await app.SaveChangesAsync();
            }
            else
            {
                await AddToDatabase(new RefreshTokenInfo() { Token = refreshToken, UserId = applicationUser.Id });
            }
            return new LoginResponse(true, "Login successfully", jwt, refreshToken);
        }

        private string GenerateToken(ApplicationUser user, string role)
        {
            var jwtProvider = new JWTProvider(config.Value!.Key!);
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Fullname!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, role!),
            };

            return jwtProvider.GenerateJWT(config.Value.Issuer, config.Value.Audience, claims, DateTime.Now.AddMinutes(5));
        }

        private async Task<UserRole> FindUserRole(int userId) => await app.UserRoles.FirstOrDefaultAsync(x => x.UserId == userId);
        private async Task<SystemRole> FindRoleName(int roleId) => await app.SystemRoles.FirstOrDefaultAsync(x => x.Id == roleId);

        //調用 RandomNumberGenerator.GetBytes(64) 產生一隨機數轉換並傳回字串
        private string GenerateRefreshToken() => Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        private async Task<ApplicationUser> FindUserByEmail(string email)
        {
            return await app.ApplicationUsers.FirstOrDefaultAsync(x=>x.Email!.ToLower()!.Equals(email!.ToLower()!));
        }

        private async Task<T> AddToDatabase<T>(T model)
        {
            var res = app.Add(model!);
            await app.SaveChangesAsync();
            return (T)res.Entity;
        }

        public async Task<LoginResponse> RefreshTokenAsync(RefreshToken token)
        {
            if (token is null) return new LoginResponse(false, "Model is empty");

            //查詢資料庫 RefreshTokenInfos資料表
            var findToken = await app.RefreshTokenInfos.FirstOrDefaultAsync(x => x.Token!.Equals(token.Token));
            if (findToken is null) return new LoginResponse(false, "Invalid RefreshToken");

            //由 findToken 查詢結果找出映射關係的 UserId 使用者資料
            var user = await app.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == findToken.UserId);
            if (user is null) return new LoginResponse(false, "User not found, no refresh token is generated");

            //建立 JWT
            var userRole = await FindUserRole(user.Id); //查詢 user-role表 結果得出user的 roleId
            var roleName = await FindRoleName(userRole.RoleId);//查詢 SystemRole表 結果得出 role 的名稱
            string jwt = GenerateToken(user, roleName.Name!);

            //用來做為刷新這個 user 上次存取時儲存舊的 refresh token
            string refreshToken = GenerateRefreshToken();

            var updateRefreshToken = await app.RefreshTokenInfos.FirstOrDefaultAsync(x => x.UserId == user.Id);
            if (updateRefreshToken is null) return new LoginResponse(false, "Refresh token could not be generated because user had not sign in.");

            updateRefreshToken.Token = refreshToken;
            await app.SaveChangesAsync();
            return new LoginResponse(true, "Token refresh successfully", jwt, refreshToken);
        }

        public async Task<List<ManageUser>> GetUsers()
        {
            var allUsers = await GetApplicationUsers();
            var allUserRoles = await UserRoles();
            var allRoles = await SystemRoles();

            if (allUsers.Count == 0 || allUserRoles.Count == 0) return null;
            var users = new List<ManageUser>();
            foreach(var user in allUsers)
            {
                var userRole = allUserRoles.FirstOrDefault(x => x.UserId == user.Id);
                var roleName = allRoles.FirstOrDefault(x => x.Id == userRole!.RoleId);
                users.Add(new ManageUser() { UserId = user.Id, Name = user.Fullname!, Email = user.Email!, Role = roleName!.Name });
            }
            return users;
        }

        public async Task<GeneralResponse> UpdateUser(ManageUser user)
        {
            var getRole = (await SystemRoles()).FirstOrDefault(x => x.Name!.Equals(user.Role));
            var userRole = await app.UserRoles.FirstOrDefaultAsync(x => x.UserId == user.UserId);
            userRole!.RoleId = getRole!.Id;
            await app.SaveChangesAsync();
            return new GeneralResponse(true, "User role updated successfuuly");
        }

        public async Task<List<SystemRole>> GetRoles() => await SystemRoles();

        public async Task<GeneralResponse> DeleteUser(int id)
        {
            var user = await app.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null) return new GeneralResponse(false, "No such user");
            app.ApplicationUsers.Attach(user!).State = EntityState.Deleted;
            await app.SaveChangesAsync();
            return new GeneralResponse(true, "User deleted successfully");
        }

        private async Task<List<SystemRole>> SystemRoles() => await app.SystemRoles.AsNoTracking().ToListAsync();
        private async Task<List<UserRole>> UserRoles() => await app.UserRoles.AsNoTracking().ToListAsync();
        private async Task<List<ApplicationUser>> GetApplicationUsers() => await app.ApplicationUsers.AsNoTracking().ToListAsync();

        public async Task<string> GetUserImage(int id)
        {
            return (await GetApplicationUsers()).FirstOrDefault(x => x.Id == id)!.Image;
        }

        public async Task<bool> UpdateProfile(UserProfile profile)
        {
            var user = await app.ApplicationUsers.FirstOrDefaultAsync(x => x.Id == int.Parse(profile.Id));
            user!.Email = profile.Email;
            user.Fullname = profile.Name;
            user.Image = profile.Image;
            await app.SaveChangesAsync();
            return true;
        }
    }
}
