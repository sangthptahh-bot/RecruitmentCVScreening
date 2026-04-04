using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;

namespace RecruitmentCVScreening.WinForms.CVProcessing.FileReaders
{
    public class PdfCVReader : ICVReader
    {
        public string ReadText(string filePath)
        {
            var text = new StringBuilder();

            using (var document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    text.AppendLine(page.Text);
                }
            }

            return text.ToString();
        }
    }
}
