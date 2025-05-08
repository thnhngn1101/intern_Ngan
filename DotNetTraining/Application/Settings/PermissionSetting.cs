using Common.Application.Models;
using Common.Application.Settings;
using Dapper;
using System.Data;

public class PermissionSetting : BasePermissionSetting
{
    private readonly IDbConnection _dbConnection;
    private Dictionary<string, List<string>> _apiPermissions = new();
    public PermissionSetting()
    {

    }
    public PermissionSetting(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
        LoadPermissionsFromDatabase().Wait(); // Wait for the async method to complete
    }
    public static async Task<PermissionSetting> CreateAsync(IDbConnection dbConnection)
    {
        var permissionSetting = new PermissionSetting(dbConnection);
        permissionSetting._apiPermissions = await permissionSetting.LoadPermissionsFromDatabaseAsync();
        return permissionSetting;
    }

    public override Dictionary<string, List<string>> ApiPermissions
    {
        get => _apiPermissions;
        // Notice that we're not providing a setter here
    }

    private async Task LoadPermissionsFromDatabase()
    {
        _apiPermissions = await LoadPermissionsFromDatabaseAsync();
    }

    private async Task<Dictionary<string, List<string>>> LoadPermissionsFromDatabaseAsync()
    {
        var apiPermissions = new Dictionary<string, List<string>>();

        string query = @"
                            SELECT 
                                CONCAT(f.APIMethod, ' - ', f.APIURI) AS ApiEndpoint, 
                                r.Name AS RoleName
                            FROM 
                                feature_roles fr
                            JOIN 
                                features f ON fr.FeatureID = f.Id
                            JOIN 
                                roles r ON fr.RoleID = r.Id;
                            ";

        var featureRoles = await _dbConnection.QueryAsync<dynamic>(query);
        foreach (var fr in featureRoles)
        {
            string apiEndpoint = (string)fr.ApiEndpoint;
            string roleName = (string)fr.RoleName;
            apiPermissions[apiEndpoint] = new List<string>{roleName, "guest"};

        }
            return apiPermissions;
    }

    public void SavePermissionsToDatabase()
    {
        foreach (var apiPermission in _apiPermissions)
        {
            var parts = apiPermission.Key.Split(" - ");
            var httpMethod = parts[0];
            var endpoint = parts[1];

            foreach (var role in apiPermission.Value)
            {
                _dbConnection.Execute(
                    "INSERT INTO ApiPermissions (HttpMethod, Endpoint, Role) VALUES (@HttpMethod, @Endpoint, @Role)",
                    new { HttpMethod = httpMethod, Endpoint = endpoint, Role = role });
            }
        }
    }
}
