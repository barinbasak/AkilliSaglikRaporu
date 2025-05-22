using AkilliSaglikRaporu.Data;
using AkilliSaglikRaporu.Models;
using AkilliSaglikRaporu.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AkilliSaglikRaporu.Controllers
{
    public class HealthReportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ISmartContractService _smartContractService;
        private readonly IIpfsService _ipfsService;
        private readonly ILogger<HealthReportController> _logger;

        public HealthReportController(
            ApplicationDbContext context,
            ISmartContractService smartContractService,
            IIpfsService ipfsService,
            ILogger<HealthReportController> logger)
        {
            _context = context;
            _smartContractService = smartContractService;
            _ipfsService = ipfsService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var userAddress = await _smartContractService.GetUserAddressFromMetaMaskAsync();
                var reports = await _context.HealthReports
                    .Where(r => r.UserAddress == userAddress)
                    .Include(r => r.Shares)
                    .ToListAsync();

                return View(reports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Raporlar listelenirken hata oluştu");
                return View(new List<HealthReport>());
            }
        }

        public IActionResult Create()
        {
            return View(new HealthReportViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HealthReportViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Kullanıcı adresini MetaMask'tan al
                model.UserAddress = await _smartContractService.GetUserAddressFromMetaMaskAsync();

                // Dosyayı IPFS'e yükle
                string ipfsHash = await _ipfsService.UploadFileToIpfsAsync(model.ReportFile);

                // Raporu veritabanına kaydet
                var healthReport = new HealthReport
                {
                    Title = model.Title,
                    Description = model.Description,
                    IpfsHash = ipfsHash,
                    UploadDate = DateTime.Now,
                    UserAddress = model.UserAddress
                };

                await _context.HealthReports.AddAsync(healthReport);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Sağlık raporu başarıyla yüklendi.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rapor oluşturulurken hata oluştu");
                ModelState.AddModelError(string.Empty, "Rapor yüklenirken bir hata oluştu. Lütfen tekrar deneyin.");
                return View(model);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var report = await _context.HealthReports
                .Include(r => r.Shares)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (report == null)
            {
                return NotFound();
            }

            return View(report);
        }

        public async Task<IActionResult> Share(int id)
        {
            var report = await _context.HealthReports.FindAsync(id);
            if (report == null)
            {
                return NotFound();
            }

            var viewModel = new HealthReportShareViewModel { HealthReportId = id };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Share(HealthReportShareViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var report = await _context.HealthReports.FindAsync(model.HealthReportId);
                if (report == null)
                {
                    return NotFound();
                }

                // Smart contract'a rapor paylaşımı için bilgi gönder
                bool contractResult = await _smartContractService.StoreHealthReportAsync(
                    report.UserAddress,
                    report.IpfsHash,
                    model.DoctorAddress,
                    model.ExpiryDate);

                if (!contractResult)
                {
                    ModelState.AddModelError(string.Empty, "Blockchain'e bilgi yazılırken bir hata oluştu.");
                    return View(model);
                }

                // Veritabanına paylaşım bilgisini kaydet
                var share = new HealthReportShare
                {
                    HealthReportId = model.HealthReportId,
                    DoctorAddress = model.DoctorAddress,
                    ExpiryDate = model.ExpiryDate,
                    CreatedDate = DateTime.Now
                };

                await _context.HealthReportShares.AddAsync(share);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Rapor başarıyla doktorla paylaşıldı.";
                return RedirectToAction(nameof(Details), new { id = model.HealthReportId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rapor paylaşılırken hata oluştu");
                ModelState.AddModelError(string.Empty, "Paylaşım işlemi sırasında bir hata oluştu. Lütfen tekrar deneyin.");
                return View(model);
            }
        }

        public async Task<IActionResult> ViewAsDoctor()
        {
            try
            {
                // Doktor adresini MetaMask'tan al
                var doctorAddress = await _smartContractService.GetUserAddressFromMetaMaskAsync();
                
                // Doktora paylaşılan ve süresi geçmemiş raporları getir
                var sharedReports = await _context.HealthReportShares
                    .Where(s => s.DoctorAddress == doctorAddress && s.ExpiryDate > DateTime.Now)
                    .Include(s => s.HealthReport)
                    .Select(s => s.HealthReport)
                    .ToListAsync();

                return View(sharedReports);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Doktor raporları listelenirken hata oluştu");
                return View(new List<HealthReport>());
            }
        }

        public async Task<IActionResult> ViewReport(int id)
        {
            try
            {
                var report = await _context.HealthReports.FindAsync(id);
                if (report == null)
                {
                    return NotFound();
                }

                // Doktor adresini al
                var doctorAddress = await _smartContractService.GetUserAddressFromMetaMaskAsync();

                // Smart contract üzerinden erişim kontrolü yap
                bool hasAccess = await _smartContractService.IsValidAccessAsync(doctorAddress, report.IpfsHash);
                if (!hasAccess)
                {
                    return Forbid("Bu rapora erişim yetkiniz bulunmamaktadır.");
                }

                // IPFS'ten rapor içeriğini al
                var reportContent = await _ipfsService.GetFileFromIpfsAsync(report.IpfsHash);
                
                // Dosya tipine göre ContentType belirle (örnek)
                string contentType = "application/pdf"; // Gerçek uygulamada dosya tipine göre değişmeli

                return File(reportContent, contentType, $"{report.Title}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rapor görüntülenirken hata oluştu");
                return RedirectToAction(nameof(ViewAsDoctor));
            }
        }
    }
} 