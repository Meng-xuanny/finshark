using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace api.Dtos.Stock
{
    public class StockPatchDto
    {
        [Required]
        [Range(1, 1000000000)]
        public decimal? Purchase { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Company Name cannot be over 10 over characters")]
        public string? CompanyName { get; set; }=String.Empty;
    }
}