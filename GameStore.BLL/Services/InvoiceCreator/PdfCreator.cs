using GameStore.BLL.Interfaces;
using GameStore.Domain.Entities.CoreEntities;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace GameStore.BLL.Services.InvoiceCreator;

public class PdfCreator(IDateTimeProvider timeProvider) : IPdfCreator
{
    private const int LeftPadding = 50;

    public byte[] GenerateInvoicePdf(Order order, int daysValid)
    {
        using var document = new PdfDocument();
        document.Info.Title = "Invoice";
        var page = document.AddPage();
        var gfx = XGraphics.FromPdfPage(page);
        var font = new XFont("Verdana", 12);

        gfx.DrawString($"Total sum: {order.TotalSum}", font, XBrushes.Black, new XRect(LeftPadding, 20, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
        gfx.DrawString($"User ID: {order.CustomerId}", font, XBrushes.Black, new XRect(LeftPadding, 40, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);
        gfx.DrawString($"Order ID: {order.Id}", font, XBrushes.Black, new XRect(LeftPadding, 60, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

        var creationDate = timeProvider.UtcNow;
        gfx.DrawString($"Creation Date: {creationDate:yyyy-MM-dd}", font, XBrushes.Black, new XRect(LeftPadding, 80, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

        var validityDate = creationDate.AddDays(daysValid);
        gfx.DrawString($"Validity Date: {validityDate:yyyy-MM-dd}", font, XBrushes.Black, new XRect(LeftPadding, 100, page.Width.Point, page.Height.Point), XStringFormats.TopLeft);

        using var stream = new MemoryStream();
        document.Save(stream, false);
        return stream.ToArray();
    }
}
