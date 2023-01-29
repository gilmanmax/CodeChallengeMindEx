using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace CodeChallenge.Models
{
    /// <summary>
    /// Represents an employee's compensation
    /// </summary>

    public class Compensation
    {
        [Key]
        public int CompensationId { get; set; }
        [Required]
        public string EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public Employee Employee { get; set; }
        [Required]
        public decimal Salary { get; set; }
        [Required]
        public DateTime EffectiveDate { get; set; }
    }
}
