using System.ComponentModel.DataAnnotations;

namespace AkilliSaglikRaporu.Models
{
    public class HealthReport
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lütfen rapor başlığı giriniz.")]
        [Display(Name = "Rapor Başlığı")]
        public string Title { get; set; } = string.Empty;
        
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
        
        [Display(Name = "IPFS Hash")]
        public string IpfsHash { get; set; } = string.Empty;
        
        [Display(Name = "Yükleme Tarihi")]
        public DateTime UploadDate { get; set; } = DateTime.Now;
        
        [Display(Name = "Kullanıcı Adresi")]
        public string UserAddress { get; set; } = string.Empty;
        
        public List<HealthReportShare> Shares { get; set; } = new List<HealthReportShare>();
    }
} 