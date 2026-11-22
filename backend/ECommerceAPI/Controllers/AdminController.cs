using ECommerceAPI.Data;
using ECommerceAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ECommerceAPI.Controllers;

/// <summary>
/// Admin-only endpoints. Every route requires Role = "Admin" in the JWT.
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
{
    private readonly IDbContext _db;

    public AdminController(IDbContext db) => _db = db;

    // ── Users ─────────────────────────────────────────────────────────────────

    /// <summary>Returns all registered users.</summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        await using var conn = await _db.OpenAsync();
        const string sql = @"
            SELECT Id, FirstName, LastName, Email, Role, CreatedAt
            FROM   Users
            ORDER  BY CreatedAt DESC";

        var list = new List<UserAdminDto>();
        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
            list.Add(new UserAdminDto(
                reader.GetInt32(0),
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetDateTime(5)
            ));

        return Ok(list);
    }

    /// <summary>Changes the role of a user ('Customer' or 'Admin').</summary>
    [HttpPut("users/{id:int}/role")]
    public async Task<IActionResult> ChangeRole(int id, [FromBody] ChangeRoleRequest req)
    {
        if (req.Role is not "Customer" and not "Admin")
            return BadRequest(new { message = "Role must be 'Customer' or 'Admin'." });

        await using var conn = await _db.OpenAsync();
        const string sql = "UPDATE Users SET Role = @Role WHERE Id = @Id";

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@Role", req.Role);
        cmd.Parameters.AddWithValue("@Id", id);

        var rows = await cmd.ExecuteNonQueryAsync();
        return rows == 0 ? NotFound() : NoContent();
    }

    /// <summary>Deletes a user and all their associated data (cascade).</summary>
    [HttpDelete("users/{id:int}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        await using var conn = await _db.OpenAsync();

        // Orders do NOT cascade from Users — delete them first.
        // OrderItems cascade from Orders automatically.
        await using (var cmd = new SqlCommand("DELETE FROM Orders WHERE UserId = @Id", conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            await cmd.ExecuteNonQueryAsync();
        }

        await using (var cmd = new SqlCommand("DELETE FROM Users WHERE Id = @Id", conn))
        {
            cmd.Parameters.AddWithValue("@Id", id);
            var rows = await cmd.ExecuteNonQueryAsync();
            return rows == 0 ? NotFound() : NoContent();
        }
    }
}
