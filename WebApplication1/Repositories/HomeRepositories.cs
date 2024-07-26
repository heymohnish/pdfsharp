using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Signatures;
using PdfSharp;
using System.Security.Cryptography.X509Certificates;
using PdfSharp.Pdf;

namespace WebApplication1.Repositories
{
    public class HomeRepositories 
    {
        /*public void DrawAppearance(XGraphics gfx, XRect rect)
        {
            for (int i = 1; i <= 2; i++)
            {
                XImage Image = XImage.FromFile(@"C:\Users\mohnish.varatharajan\source\repos\pdfsharp\WebApplication1\wwwroot\image\sachinsign.jpg");
                XColor empty = XColor.Empty;
                string text = "Signed by Napoleon \nLocation: Paris \nDate: " + DateTime.Now.ToString();
                XFont font = new XFont("Verdana", 7.0, XFontStyleEx.Regular);
                XTextFormatter xTextFormatter = new XTextFormatter(gfx);
                int num = Image.PixelWidth / Image.PixelHeight;
                XPoint xPoint = new XPoint(0.0, 0.0);
                bool flag = Image != null;
                if (flag)
                {
                    gfx.DrawImage(Image, xPoint.X + i, xPoint.Y + i, ((5 * i) + rect.Width) / 4.0, ((5 * i) + rect.Width) / 4.0 / (double)num);
                    xPoint = new XPoint(rect.Width / 4.0, 0.0);
                }
                xTextFormatter.DrawString(text, font, new XSolidBrush(XColor.FromKnownColor(XKnownColor.Black)), new XRect((5 * i) + xPoint.X, (5 * i) + xPoint.Y, rect.Width - xPoint.X, rect.Height), XStringFormats.TopLeft);
            }

        }
        public void CallPDF()
        {
            *//*if (Capabilities.Build.IsCoreBuild)
                GlobalFontSettings.FontResolver = new FailsafeFontResolver();*//*

            CreateAndSign();
            SignExisting();
        }
        private static void CreateAndSign()
        {
            string text = "CreateAndSign8.pdf";
            XFont font = new XFont("Verdana", 10.0, XFontStyleEx.Regular);
            PdfDocument pdfDocument = new PdfDocument();
            PdfPage pdfPage = pdfDocument.AddPage();
            XGraphics xGraphics = XGraphics.FromPdfPage(pdfPage);
            XRect layoutRectangle = new XRect(0.0, 0.0, pdfPage.Width, pdfPage.Height);
            xGraphics.DrawString("Sample document", font, XBrushes.Black, layoutRectangle, XStringFormats.TopCenter);
            PdfSignatureOptions options = new PdfSignatureOptions
            {
                ContactInfo = "Contact Info",
                Location = "Paris",
                Reason = "Test signatures",
                Rectangle = new XRect(36.0, 700.0, 200.0, 50.0),
                // AppearanceHandler = new HomeRepositories()
            };
            PdfSignatureHandler pdfSignatureHandler = new PdfSignatureHandler(new BouncySigner(GetCertificate()), options);
            pdfSignatureHandler.AttachToDocument(pdfDocument);
            pdfDocument.Save(text);
            //Process.Start(text);
        }
        private static void SignExisting()
        {
            string text = string.Format("SignExisting8.pdf", new object[0]);
            PdfDocument pdfDocument = PdfReader.Open(File.OpenRead(@"C:\Users\mohnish.varatharajan\source\repos\pdfsharp\WebApplication1\pdf\dummy.pdf"));
            PdfSignatureOptions options = new PdfSignatureOptions
            {
                ContactInfo = "Contact Info",
                Location = "Paris",
                Reason = "Test signatures",
                Rectangle = new XRect(36.0, 735.0, 200.0, 50.0),
                AppearanceHandler = new HomeRepositories()
            };
            PdfSignatureHandler pdfSignatureHandler = new PdfSignatureHandler(null, options);
            pdfSignatureHandler.AttachToDocument(pdfDocument);
            pdfDocument.Save(text);
            //Process.Start(text);
        }
        private static Tuple<X509Certificate2, X509Certificate2Collection> GetCertificate()
        {
            var rawData = File.ReadAllBytes(@"C:\Users\mohnish.varatharajan\source\repos\pdfsharp\WebApplication1\key\nextlysign.pfx");
            var certificatePassword = "Dhanya123#";

            var certificate = new X509Certificate2(rawData
                , certificatePassword
                , X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);

            X509Certificate2Collection collection = new X509Certificate2Collection();
            collection.Import(rawData, certificatePassword, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);


            return Tuple.Create(certificate, collection);
        }*/
        public void hellobuddy()
        {
            var cert = new X509Certificate2(@"C:\Users\mohnish.varatharajan\source\repos\pdfsharp\WebApplication1\key\nextlysign.pfx", "Dhanya123#");
            var options = new PdfSignatureOptions
            {
                Certificate = cert,
                FieldName = "Signature-" + Guid.NewGuid().ToString("N"),
                PageIndex = 0,
                Rectangle = new XRect(120, 10, 100, 60),
                Location = "My PC",
                Reason = "Approving Rev #2",
                // Signature appearances can also consist of an image (Rectangle should be adapted to image size)
                //Image = XImage.FromFile(@"C:\Data\stamp.png")
            };

            // first signature
            //var sourceFile = IOUtility.GetAssetsPath("archives/grammar-by-example/GBE/ReferencePDFs/WPF 1.31/Table-Layout.pdf")!;
            //var targetFile = Path.Combine(Path.GetTempPath(), "AA-Signed.pdf");

            // second signature
            var sourceFile = Path.Combine(Path.GetTempPath(), "C:\\Users\\mohnish.varatharajan\\source\\repos\\pdfsharp\\WebApplication1\\SignExisting8.pdf");
            var targetFile = Path.Combine(Path.GetTempPath(), "C:\\Users\\mohnish.varatharajan\\source\\repos\\pdfsharp\\WebApplication1\\CreateAndSign8.pdf");
            File.Copy(sourceFile, targetFile, true);

            using var fs = File.Open(targetFile, FileMode.Open, FileAccess.ReadWrite);
            var signer = new PdfSigner(fs, options);
            var resultStream = signer.Sign();
            // overwrite input document
            fs.Seek(0, SeekOrigin.Begin);
            resultStream.CopyTo(fs);
        }
    }

}
