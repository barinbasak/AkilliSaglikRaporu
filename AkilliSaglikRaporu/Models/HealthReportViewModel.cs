using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AkilliSaglikRaporu.Models
{
    public class HealthReportViewModel
    {
        [Required(ErrorMessage = "Lütfen rapor başlığı giriniz.")]
        [Display(Name = "Rapor Başlığı")]
        public string Title { get; set; } = string.Empty;
        
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
        
        [Required(ErrorMessage = "Lütfen bir rapor dosyası seçiniz.")]
        [Display(Name = "Rapor Dosyası")]
        public IFormFile ReportFile { get; set; } = null!;
        
        [Display(Name = "Ethereum Adresiniz")]
        public string UserAddress { get; set; } = string.Empty;
    }

    public class HealthReportShareViewModel
    {
        public int HealthReportId { get; set; }
        
        [Required(ErrorMessage = "Lütfen doktor Ethereum adresi giriniz.")]
        [Display(Name = "Doktor Ethereum Adresi")]
        public string DoctorAddress { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Lütfen paylaşım için bitiş tarihi belirleyin.")]
        [Display(Name = "Paylaşım Bitiş Tarihi")]
        public DateTime ExpiryDate { get; set; } = DateTime.Now.AddDays(7);
    }
} 