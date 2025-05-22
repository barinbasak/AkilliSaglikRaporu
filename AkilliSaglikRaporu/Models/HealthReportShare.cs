using System.ComponentModel.DataAnnotations;

namespace AkilliSaglikRaporu.Models
{
    public class HealthReportShare
    {
        public int Id { get; set; }
        
        public int HealthReportId { get; set; }
        public HealthReport? HealthReport { get; set; }
        
        [Required(ErrorMessage = "Lütfen doktor Ethereum adresi giriniz.")]
        [Display(Name = "Doktor Ethereum Adresi")]
        public string DoctorAddress { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Lütfen paylaşım için bitiş tarihi belirleyin.")]
        [Display(Name = "Paylaşım Bitiş Tarihi")]
        public DateTime ExpiryDate { get; set; }
        
        [Display(Name = "Paylaşım Oluşturma Tarihi")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Aktif Mi?")]
        public bool IsActive => DateTime.Now <= ExpiryDate;
    }
} 