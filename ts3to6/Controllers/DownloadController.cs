using Microsoft.AspNetCore.Mvc;
using ts3to6.Services;

namespace ts3to6.Controllers;

/// <summary>
/// Controller responsible for serving the generated TeamSpeak 6 database files.
/// </summary>
[ApiController]
[Route("dl")]
public class DownloadController : ControllerBase
{
    private readonly DownloadCache _cache;

    public DownloadController(DownloadCache cache)
    {
        _cache = cache;
    }

    /// <summary>
    /// Serves the generated <c>tsserver.sqlitedb</c> file for a given conversion ID.
    /// </summary>
    /// <param name="id">The unique ID returned by the migration service.</param>
    /// <param name="filename">The filename (required for the URL structure, but the actual file is always served as tsserver.sqlitedb).</param>
    /// <returns>The file stream or 404 if the session has expired.</returns>
    [HttpGet("{id}/{filename}")]
    public IActionResult GetFile(string id, string filename)
    {
        byte[]? data = _cache.Get(id);

        if (data == null)
        {
            return NotFound("Le fichier a expiré ou n'existe pas. Veuillez recommencer la conversion.");
        }

        return File(
            data,
            "application/octet-stream",
            "tsserver.sqlitedb",
            enableRangeProcessing: false
        );
    }
}
