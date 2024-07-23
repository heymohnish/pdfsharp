
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using PdfSharp.Fonts;
using PdfSharp.Pdf.Annotations;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf.Signatures;
using PdfSharp.Pdf;
using PdfSharp.Snippets.Font;
using PdfSharp;
using System.Security.Cryptography.X509Certificates;

namespace WebApplication1.Repositories
{
    public class HomeRepositories : IAnnotationAppearanceHandler
    {
        private XImage Image = XImage.FromFile(@"C:\Users\mohnish.varatharajan\source\repos\pdfsharp\WebApplication1\wwwroot\image\sachinsign.jpg");

        public void DrawAppearance(XGraphics gfx, XRect rect)
        {
            XColor empty = XColor.Empty;
            string text = "Signed by Napoleon \nLocation: Paris \nDate: " + DateTime.Now.ToString();
            XFont font = new XFont("Verdana", 7.0, XFontStyleEx.Regular);
            XTextFormatter xTextFormatter = new XTextFormatter(gfx);
            int num = this.Image.PixelWidth / this.Image.PixelHeight;
            XPoint xPoint = new XPoint(0.0, 0.0);
            bool flag = this.Image != null;
            if (flag)
            {
                gfx.DrawImage(this.Image, xPoint.X, xPoint.Y, rect.Width / 4.0, rect.Width / 4.0 / (double)num);
                xPoint = new XPoint(rect.Width / 4.0, 0.0);
            }
            xTextFormatter.DrawString(text, font, new XSolidBrush(XColor.FromKnownColor(XKnownColor.Black)), new XRect(xPoint.X, xPoint.Y, rect.Width - xPoint.X, rect.Height), XStringFormats.TopLeft);
        }
        public void CallPDF()
        {
            if (Capabilities.Build.IsCoreBuild)
                GlobalFontSettings.FontResolver = new FailsafeFontResolver();

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
                //AppearanceHandler = new Program.SignAppearenceHandler()
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
            PdfSignatureHandler pdfSignatureHandler = new PdfSignatureHandler(new BouncySigner(GetCertificate()), options);
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
        }

    }

}
