﻿using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModel.UploadImage;

public class UploadImageViewModel
{
    [Required(ErrorMessage = "Imagem inválida")]
    public string Base64Image { get; set; }
}