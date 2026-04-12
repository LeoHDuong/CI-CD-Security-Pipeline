// TEST FILE — intentionally vulnerable code to trigger SAST findings
// DO NOT use in production

using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace MyApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VulnerableController : ControllerBase
{
    private readonly string _connectionString =
        "Server=localhost;Database=MyApp;User Id=sa;Password=P@ssw0rd123!;";

    // Vulnerability 1: SQL Injection — string concatenation into SqlCommand
    // Triggers: csharp.lang.security.sqli.csharp-sqli (p/csharp)
    [HttpGet("search")]
    public IActionResult Search([FromQuery] string username)
    {
        using var conn = new SqlConnection(_connectionString);
        conn.Open();
        var query = "SELECT * FROM Users WHERE Username = '" + username + "'";
        using var cmd = new SqlCommand(query, conn);
        using var reader = cmd.ExecuteReader();
        var results = new List<string>();
        while (reader.Read())
            results.Add(reader.GetString(0));
        return Ok(results);
    }

    // Vulnerability 2: Insecure Deserialization via BinaryFormatter
    // Triggers: csharp.lang.security.insecure-deserialization.binary-formatter (p/csharp)
    [HttpPost("deserialize")]
    public IActionResult Deserialize()
    {
        using var ms = new MemoryStream();
        Request.Body.CopyTo(ms);
        ms.Position = 0;
#pragma warning disable SYSLIB0011
        var formatter = new BinaryFormatter();
        var obj = formatter.Deserialize(ms);
#pragma warning restore SYSLIB0011
        return Ok(obj?.ToString());
    }

    // Vulnerability 3: Weak hash (MD5) used for password storage
    // Triggers: csharp.lang.security.crypto.weak-hash.csharp-weak-hash (p/csharp)
    [HttpPost("register")]
    public IActionResult Register([FromBody] string password)
    {
        using var md5 = MD5.Create();
        var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hashHex = Convert.ToHexString(hash);
        // store hashHex in DB ...
        return Ok(hashHex);
    }

    // Vulnerability 4: XXE — XmlDocument loaded without disabling external entities
    // Triggers: csharp.dotnet.security.xxe.xmldocument-no-xmlresolver (p/owasp-top-ten)
    [HttpPost("parse-xml")]
    public IActionResult ParseXml([FromBody] string xmlContent)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xmlContent);
        return Ok(doc.InnerText);
    }

    // Vulnerability 5: Weak hash (SHA1) used for integrity check
    // Triggers: csharp.lang.security.crypto.weak-hash.csharp-weak-hash (p/csharp)
    [HttpGet("checksum")]
    public IActionResult Checksum([FromQuery] string data)
    {
        using var sha1 = SHA1.Create();
        var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Ok(Convert.ToHexString(hash));
    }
}
