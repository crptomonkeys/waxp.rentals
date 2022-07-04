using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SkiaSharp;
using SkiaSharp.QrCode;
using SkiaSharp.QrCode.Models;
using static WaxRentalsWeb.Config.Constants;

namespace WaxRentalsWeb.Pages.QR
{
    public abstract class QRPageModel : PageModel
    {

        protected FileContentResult GenerateQRCode(string value)
        {
            var code = new QRCodeGenerator().CreateQrCode(value, ECCLevel.Q, quietZoneSize: 0);
            var icon = new IconData { Icon = SKBitmap.Decode(Images.Logo), IconSizePercent = 30 };

            using var surface = SKSurface.Create(new SKImageInfo(Images.Size, Images.Size));
            var canvas = surface.Canvas;
            canvas.Render(code, Images.Size, Images.Size, SKColor.Parse("FFFFFF"), SKColor.Parse("000000"), icon);

            using var snapshot = surface.Snapshot();
            using var data = snapshot.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream();
            data.SaveTo(stream);
            return File(stream.ToArray(), "image/png");
        }

    }
}
