using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Models;

public partial class Movie
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required!!!")]
    public string Title { get; set; } = null!;

    [MaxLength(500,ErrorMessage = "Maximum description length is 500 chars")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "You must specify the genre!")]
    public int GenreId { get; set; }

    public virtual Genre? Genre { get; set; } = null;

    public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();

    [NotMapped]
    public bool IsValid { get; set; } = true;
}
