// TEST FILE — intentionally vulnerable code to trigger SAST findings
// DO NOT use in production

using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VulnerableController : ControllerBase
{
    private readonly string _connectionString = "Server=localhost;Database=MyApp;User Id=sa;Password=P@ssw0rd123!;";

    // Vulnerability 1: SQL Injection — user input concatenated directly into query
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string username)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();

        // semgrep: csharp.lang.security.sqli.direct-string-concat
        var query = "SELECT * FROM Users WHERE Username = '" + username + "'";
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();

        var results = new List<string>();
        while (reader.Read())
            results.Add(reader.GetString(0));

        return Ok(results);
    }

    // Vulnerability 2: Hard-coded credentials
    [HttpGet("admin")]
    public IActionResult Admin([FromQuery] string password)
    {
        // semgrep: generic.secrets.security.detected-generic-password
        const string adminPassword = "admin123";

        if (password == adminPassword)
            return Ok("Welcome, admin!");

        return Unauthorized();
    }

    // Vulnerability 3: Path traversal — user-supplied filename read directly
    [HttpGet("file")]
    public IActionResult GetFile([FromQuery] string filename)
    {
        // semgrep: csharp.lang.security.path-traversal
        var path = Path.Combine("/var/app/uploads", filename);
        var content = System.IO.File.ReadAllText(path);
        return Content(content);
    }

    // Vulnerability 4: Command injection via Process.Start with user input
    [HttpGet("ping")]
    public IActionResult Ping([FromQuery] string host)
    {
        // semgrep: csharp.lang.security.os-command-injection
        var process = new System.Diagnostics.Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c ping " + host;
        process.Start();
        process.WaitForExit();
        return Ok("Done");
    }
}
