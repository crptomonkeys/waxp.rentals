using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QRCoder;
using static WaxRentalsWeb.Config.Constants;

namespace WaxRentalsWeb.Pages.QR
{
    public abstract class QRPageModel : PageModel
    {

        protected FileContentResult GenerateQRCode(string value)
        {
            var data = QRCodeGenerator.GenerateQrCode(value, QRCodeGenerator.ECCLevel.Q);
            var code = new QRCode(data);
            var qr = code.GetGraphic(
                (int)Math.Ceiling((decimal)Images.Size / (decimal)data.ModuleMatrix.Count) + 1,
                Color.Black,
                Color.White,
                icon: new Bitmap(Images.Logo),
                iconSizePercent: 30,
                drawQuietZones: false);

            byte[] image;
            using (var stream = new MemoryStream())
            {
                qr.Save(stream, ImageFormat.Png);
                image = stream.ToArray();
            }
            return File(image, "image/png");
        }

    }
}
