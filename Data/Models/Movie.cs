using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Data.Models;

public partial class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public int GenreId { get; set; }

    public virtual Genre? Genre { get; set; } = null;

    public virtual ICollection<ShowTime> ShowTimes { get; set; } = new List<ShowTime>();
}
