using Microsoft.AspNetCore.Mvc;
/*using Org.BouncyCastle.Crypto;
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
using System.Collections.Generic;
using System.IO;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;
using System.Security.Cryptography.X509Certificates;*/
using PdfSharp.Pdf;
using PdfSharp.Pdf.Advanced;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Cms;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;
using Org.BouncyCastle.X509.Store;
using Org.BouncyCastle.Crypto;
using System.Collections;
namespace WebApplication1.Repositories
{
    public class HomeRepositories
    {
        public byte[] SignPdf(PdfDocument document,  Org.BouncyCastle.X509.X509Certificate certificate, AsymmetricKeyParameter privateKey, string outputPath)
        {
            // Create the signature dictionary
            PdfDictionary sigDict = new PdfDictionary();
            sigDict.Elements["/Type"] = new PdfName("/Sig");
            sigDict.Elements["/Filter"] = new PdfName("/Adobe.PPKLite");
            sigDict.Elements["/SubFilter"] = new PdfName("/adbe.pkcs7.detached");
            PdfArray byteRangeArray = new PdfArray();
            byteRangeArray.Elements.Add(new PdfInteger(0));
            byteRangeArray.Elements.Add(new PdfInteger(0));
            byteRangeArray.Elements.Add(new PdfInteger(0));
            byteRangeArray.Elements.Add(new PdfInteger(0));
            sigDict.Elements["/ByteRange"] = byteRangeArray;
            // Initialize Contents with placeholder signature content
            string placeholderContent = new string('0', 8192 / 2); // Hex encoding means each byte is represented by 2 characters
            sigDict.Elements["/Contents"] = new PdfString(placeholderContent, PdfStringEncoding.PDFDocEncoding);
            sigDict.Elements["/Reason"] = new PdfString("Document signed");
            sigDict.Elements["/M"] = new PdfString(DateTime.UtcNow.ToString("yyyyMMddHHmmssZ"));
            PdfDictionary acroForm = document.AcroForm.Elements.ContainsKey("/Sig") ? document.AcroForm.Elements.GetDictionary("/Sig") : new PdfDictionary();
            acroForm.Elements["/V"] = sigDict;
            document.AcroForm.Elements["/Sig"] = acroForm;
            // Save the PDF to memory
            //MemoryStream ms = new MemoryStream();
            try
            {
                // Save the document to a MemoryStream
                using (MemoryStream ms = new MemoryStream())
                {
                    document.Save(ms, false);
                    // Extract content bytes
                    /*List<byte> contentBytesList = new List<byte>();
                    foreach (PdfPage page in document.Pages)
                    {
                        PdfDictionary contentDict = (PdfDictionary)page.Elements.GetDictionary("/Contents");
                        PdfItem contentsItem = contentDict.Elements.GetObject("/Contents");

                        if (contentsItem is PdfArray)
                        {
                            PdfArray contentsArray = (PdfArray)contentsItem;
                            foreach (PdfReference reference in contentsArray)
                            {
                                PdfDictionary streamDict = (PdfDictionary)reference.Value;
                                byte[] streamBytes = streamDict.Stream.Value;
                                contentBytesList.AddRange(streamBytes);
                            }
                        }
                        else if (contentsItem is PdfReference)
                        {
                            PdfDictionary streamDict = (PdfDictionary)((PdfReference)contentsItem).Value;
                            byte[] streamBytes = streamDict.Stream.Value;
                            contentBytesList.AddRange(streamBytes);
                        }
                    }*/

                    byte[] contentBytes = ms.ToArray();
                    // Create the CMS signed data
                    Org.BouncyCastle.Cms.CmsSignedDataGenerator generator = new Org.BouncyCastle.Cms.CmsSignedDataGenerator();
                    generator.AddSigner(privateKey, certificate, Org.BouncyCastle.Cms.CmsSignedDataGenerator.DigestSha256);
                    /*Org.BouncyCastle.X509.X509Certificate bcCertificate = new X509CertificateParser().ReadCertificate(certificate.GetEncoded());*/
                    /*Org.BouncyCastle.Utilities.Collections.IList bcCertList = new ArrayList();
                    bcCertList.Add(bcCertificate);*/
                    X509CertificateParser parser = new X509CertificateParser();
                    Org.BouncyCastle.X509.X509Certificate bcCertificate = parser.ReadCertificate(certificate.GetEncoded());

                    IList<Org.BouncyCastle.X509.X509Certificate> certList = new List<Org.BouncyCastle.X509.X509Certificate> { bcCertificate };
                    IX509Store certStore = X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters((ICollection)certList));
                    generator.AddCertificates(certStore);
                    //generator.AddCertificates(new X509CertificateParser().ReadCertificate(certificate.GetEncoded()));
                    Org.BouncyCastle.Cms.CmsProcessableByteArray content = new Org.BouncyCastle.Cms.CmsProcessableByteArray(contentBytes);
                    Org.BouncyCastle.Cms.CmsSignedData signedData = generator.Generate(content, true);

                    // Replace the placeholder with the actual signature
                    byte[] signature = signedData.GetEncoded();
                    byte[] paddedSignature = new byte[signature.Length];
                    Array.Copy(signature, 0, paddedSignature, 0, signature.Length);

                    // Update the signature dictionary with the actual signature
                    /*sigDict.Elements["/Contents"] = new PdfString(paddedSignature, true);

                    // Update the ByteRange
                    sigDict.Elements["/ByteRange"] = new PdfArray(new int[] { 0, paddedSignature.Length, paddedSignature.Length, (int)ms.Length - paddedSignature.Length });*/

                    // Save the signed PDF
                    ms.Position = 0;
                    //fileBytes = ms.ToArray();

                    //File.WriteAllBytes(outputPath, ms.ToArray());
                    
                    return ms.ToArray();
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions related to file operations or PDF modifications
                Console.WriteLine("An error occurred: " + ex.Message);
                throw;
            }


            //document.Save(outputPath);
        }
     /*   public byte[] SignPdf1(PdfDocument document, X509Certificate2 certificate, AsymmetricKeyParameter privateKey, string outputPath)
        {
            // Create the signature dictionary
            PdfDictionary sigDict = new PdfDictionary();
            sigDict.Elements["/Type"] = new PdfName("/Sig");
            sigDict.Elements["/Filter"] = new PdfName("/Adobe.PPKLite");
            sigDict.Elements["/SubFilter"] = new PdfName("/adbe.pkcs7.detached");

            // Initialize ByteRange with placeholder values
            PdfArray byteRangeArray = new PdfArray();
            byteRangeArray.Elements.Add(new PdfInteger(0));
            byteRangeArray.Elements.Add(new PdfInteger(0));
            byteRangeArray.Elements.Add(new PdfInteger(0));
            byteRangeArray.Elements.Add(new PdfInteger(0));
            sigDict.Elements["/ByteRange"] = byteRangeArray;

            // Initialize Contents with placeholder signature content
            string placeholderContent = new string('0', 8192 * 2); // Each byte is represented by 2 characters in hex
            sigDict.Elements["/Contents"] = new PdfString(placeholderContent, PdfStringEncoding.PDFDocEncoding);

            sigDict.Elements["/Reason"] = new PdfString("Document signed");
            sigDict.Elements["/M"] = new PdfString(DateTime.UtcNow.ToString("yyyyMMddHHmmssZ"));

            // Add the signature dictionary to the document's AcroForm
            PdfDictionary acroForm = document.AcroForm.Elements.ContainsKey("/Sig") ? document.AcroForm.Elements.GetDictionary("/Sig") : new PdfDictionary();
            acroForm.Elements["/V"] = sigDict;
            document.AcroForm.Elements["/Sig"] = acroForm;

            // Save the document to a MemoryStream
            using (MemoryStream ms = new MemoryStream())
            {
                document.Save(ms, false);

                // Get the content bytes
                byte[] contentBytes = ms.ToArray();

                // Create the CMS signed data
                CmsSignedDataGenerator generator = new CmsSignedDataGenerator();
                generator.AddSigner(privateKey, DotNetUtilities.FromX509Certificate(certificate), CmsSignedDataGenerator.DigestSha256);

                X509CertificateParser parser = new X509CertificateParser();
                Org.BouncyCastle.X509.X509Certificate bcCertificate = parser.ReadCertificate(certificate.RawData);

                IList<Org.BouncyCastle.X509.X509Certificate> certList = new List<Org.BouncyCastle.X509.X509Certificate> { bcCertificate };
                IX509Store certStore = X509StoreFactory.Create("Certificate/Collection", new X509CollectionStoreParameters((ICollection)certList));
                generator.AddCertificates(certStore);

                CmsProcessableByteArray content = new CmsProcessableByteArray(contentBytes);
                CmsSignedData signedData = generator.Generate(content, true);

                // Replace the placeholder with the actual signature
                byte[] signature = signedData.GetEncoded();
                byte[] paddedSignature = new byte[8192];
                Array.Copy(signature, 0, paddedSignature, 0, Math.Min(signature.Length, paddedSignature.Length));

                // Calculate the ByteRange
                int contentLength = contentBytes.Length;
                int signatureOffset = contentLength - 8192 * 2 - 2; // Adjust offset based on signature length
                byteRangeArray.Elements.Clear();
                byteRangeArray.Elements.Add(new PdfInteger(0));
                byteRangeArray.Elements.Add(new PdfInteger(signatureOffset));
                byteRangeArray.Elements.Add(new PdfInteger(signatureOffset + 8192));
                byteRangeArray.Elements.Add(new PdfInteger(contentLength - (signatureOffset + 8192)));

                // Update the signature dictionary
                sigDict.Elements["/ByteRange"] = byteRangeArray;
                sigDict.Elements["/Contents"] = new PdfString(paddedSignature , PdfStringEncoding.PDFDocEncoding);

                // Save the signed PDF
                using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(contentBytes, 0, contentBytes.Length);
                }

                return ms.ToArray();
            }
        }*/
    }
  
}
