﻿namespace Task_Cat_ProMvc.Models.viewModel
{
    public class DeleteCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; } = true;
    }
} 
