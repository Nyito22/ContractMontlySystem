using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace ContractMontlySystem.Models
{
    public class Claims
    {
        public string? ClaimId { get; set; }  // Remove [Required]

        [Required(ErrorMessage = "Lecture Name is required")]
        [StringLength(100, ErrorMessage = "Lecture Name cannot be longer than 100 characters.")]
        public string LectureName { get; set; }

        [Required(ErrorMessage = "Lecture ID is required")]
        [StringLength(50, ErrorMessage = "Lecture ID cannot be longer than 50 characters.")]
        public string LectureId { get; set; }

        [Required(ErrorMessage = "Module Name is required")]
        [StringLength(100, ErrorMessage = "Module Name cannot be longer than 100 characters.")]
        public string ModuleName { get; set; }

        [Required(ErrorMessage = "Claim Period From is required")]
        [DataType(DataType.Date)]
        public DateTime ClaimFrom { get; set; }

        [Required(ErrorMessage = "Claim Period To is required")]
        [DataType(DataType.Date)]
        public DateTime ClaimTo { get; set; }

        [Required(ErrorMessage = "Hourly Wage is required")]
        [Range(0.01, double.MaxValue)]
        public decimal HourlyWage { get; set; }

        [Required(ErrorMessage = "Session Hours are required")]
        [Range(1, int.MaxValue)]
        public int SessionHours { get; set; }

        public string? SupportingDocsPath { get; set; }  // Remove [Required], filled in controller

        public IFormFile SupportingDocs { get; set; }  // For upload only

        public string Claim_Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }  // Calculated in controller
    }
}
