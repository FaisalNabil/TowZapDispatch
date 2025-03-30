using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Dispatch.Domain.Entities;
using Dispatch.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;

public class NotificationLetterService
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;

    public NotificationLetterService(ApplicationDbContext db, IWebHostEnvironment env)
    {
        _db = db;
        _env = env;
    }

    public async Task<string> Generate1stLetterAsync(Guid jobId)
    {
        var job = await _db.JobRequests.FindAsync(jobId);
        if (job == null) throw new Exception("Job not found");

        var fileName = $"{job.Id}_1st.pdf";
        var filePath = Path.Combine(_env.WebRootPath ?? "wwwroot", "letters", fileName);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

        using (var doc = new PdfDocument())
        {
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12);

            gfx.DrawString("1st Notification Letter", font, XBrushes.Black, new XRect(0, 0, page.Width, 40), XStringFormats.Center);
            gfx.DrawString($"Tow Date: {job.TowDate}", font, XBrushes.Black, new XPoint(40, 80));
            gfx.DrawString($"Vehicle: {job.Year} {job.Make} {job.Model}", font, XBrushes.Black, new XPoint(40, 110));
            gfx.DrawString($"License Plate: {job.LicensePlate}", font, XBrushes.Black, new XPoint(40, 140));
            gfx.DrawString($"Towed From: {job.AddressFrom}", font, XBrushes.Black, new XPoint(40, 170));
            gfx.DrawString($"Towed To: {job.AddressTo}", font, XBrushes.Black, new XPoint(40, 200));

            doc.Save(filePath);
        }

        // Save path to DB
        var letter = new NotificationLetter
        {
            Id = Guid.NewGuid(),
            JobRequestId = jobId,
            LetterType = "1st",
            GeneratedOn = DateTime.UtcNow,
            FilePath = filePath
        };

        _db.NotificationLetters.Add(letter);
        await _db.SaveChangesAsync();

        return filePath;
    }
}
