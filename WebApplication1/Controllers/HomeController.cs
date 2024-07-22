using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Pkcs;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using System.Diagnostics;
using WebApplication1.Models;
using WebApplication1.Repositories;
using Org.BouncyCastle.X509;

using PdfSharp.Pdf.AcroForms;
using System.Collections;
using Org.BouncyCastle.X509.Store;
using PdfSharp.Pdf.Advanced;
using Org.BouncyCastle.Utilities.Zlib;
using System;

using System.IO;using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
       // private readonly HomeRepositories _homeRepositories;

        public HomeController(ILogger<HomeController> logger)
        {

            _logger = logger;
              
        }

        public IActionResult Index()
        {
            
           //SignPDF();
            // Load the PDF
            string inputPdfPath = "C:\\Users\\mohnish.varatharajan\\source\\repos\\pdfsharp\\WebApplication1\\pdf\\aaaa.pdf";
            string outputPdfPath = "C:\\Users\\mohnish.varatharajan\\source\\repos\\pdfsharp\\WebApplication1\\pdf\\dummy-pdf_2.pdf";
            PdfDocument document = PdfReader.Open(inputPdfPath, PdfDocumentOpenMode.Modify);

            // Load the certificate
            string certPath = "C:\\Users\\mohnish.varatharajan\\source\\repos\\pdfsharp\\WebApplication1\\key\\nextlysign.pfx";
            string certPassword = "Dhanya123#";
            Pkcs12Store store = new Pkcs12Store(new FileStream(certPath, FileMode.Open, FileAccess.Read), certPassword.ToCharArray());
            string alias = null;
            foreach (string al in store.Aliases)
            {
                if (store.IsKeyEntry(al))
                {
                    alias = al;
                    break;
                }
            }

            AsymmetricKeyEntry keyEntry = store.GetKey(alias);
            X509CertificateEntry[] chain = store.GetCertificateChain(alias);

            X509Certificate certificate = chain[0].Certificate;
            AsymmetricKeyParameter privateKey = keyEntry.Key;

            // Sign the PDF



            HomeRepositories homeRepositories   = new HomeRepositories();
            byte[] fileBytes = homeRepositories.SignPdf(document, certificate, privateKey, outputPdfPath);
            return File(fileBytes, "application/pdf", "signed-api.pdf");

           // return View();
        }

        public IActionResult Privacy()
        {

            
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
       
       
    }
}
