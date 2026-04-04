using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecruitmentCVScreening.WinForms.Business.DTOs;
using RecruitmentCVScreening.WinForms.Business.Services;
using RecruitmentCVScreening.WinForms.Core.Models;
using RecruitmentCVScreening.WinForms.Core.Enums;
using RecruitmentCVScreening.WinForms.CVProcessing.FileReaders;
using RecruitmentCVScreening.WinForms.CVProcessing.Extractors;
using RecruitmentCVScreening.WinForms.Data.Tables;
namespace RecruitmentCVScreening.WinForms.Business.Services;

public class CVProcessingService
{
    public CandidateScoreDto Process(string fullName, string email, string filePath, Job job)
    {
        // 1. Đọc CV
        string extension = Path.GetExtension(filePath).ToLower();

        ICVReader reader;

        if (extension == ".docx")
        {
            reader = new DocxCVReader();
        }
        else if (extension == ".pdf")
        {
            reader = new PdfCVReader();
        }
        else
        {
            throw new Exception("Định dạng file không hỗ trợ");
        }

        string text = reader.ReadText(filePath);

        // 2. Trích xuất thông tin
        var extractor = new CVInformationExtractor();
        var cvInfo = extractor.Extract(text);

        if (string.IsNullOrWhiteSpace(cvInfo.Email))
        {
            throw new Exception("Email extracted from CV is NULL or EMPTY");
        } 

        var candidateData = new CandidateData();
        int candidateId = candidateData.GetOrCreate(
            cvInfo.FullName,
            cvInfo.Email
        );
        if (candidateId <= 0)
        {
            throw new Exception("CandidateId is INVALID (<= 0)");
        }


        // 3. Chấm điểm
        var scoringService = new ScoringService();
        var score = scoringService.Calculate(cvInfo, job);



        // 4. Lưu vào bảng Application
        var application = new RecruitmentCVScreening.WinForms.Core.Models.Application
        {
            JobId = job.Id,
            CandidateId = candidateId,
            Score = score,
            Status = score >= 60
            ? ApplicationStatus.Accepted
            : ApplicationStatus.Rejected
        };

        var appData = new ApplicationData();
        appData.Insert(application);

        // 5. Trả DTO cho UI
        return new CandidateScoreDto
        {
            FullName = cvInfo.FullName,
            Email = cvInfo.Email,
            Score = score,
            Decision = score >= 60 ? "Invite Interview" : "Reject"
        };
    }
}
