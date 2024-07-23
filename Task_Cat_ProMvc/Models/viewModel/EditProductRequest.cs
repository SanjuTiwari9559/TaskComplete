using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Task_Cat_ProMvc.Models.viewModel
{
    public class EditProductRequest
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        public string Name { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }

       
        public List<SelectListItem> Categories { get; set; } = null;
    }
}
