using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GymSystem2.Models;

public partial class Profile
{
    public decimal Profileid { get; set; }

    public string Fname { get; set; } = null!;

    public string Lname { get; set; } = null!;

    public string? Gender { get; set; }

    public string? Imagepath { get; set; }
  
    [NotMapped]
    public virtual IFormFile ImageFile { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
