using System.ComponentModel.DataAnnotations;

namespace backend.Models;

public class AddUserDetail
{
    public int Roleid{ get; set; }

    [Required(ErrorMessage = "First name is required")]
    public string Firstname { get; set; } = null!;
    
    [Required(ErrorMessage = "First name is required")]
    public string Lastname { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;
    public string Password{ get; set; } = null!;
    public string Profileimage { get; set; } = null!;

    public string Phonenumber { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Zipcode { get; set; } = null!;

    public bool Status { get; set; }

    public int Countryid { get; set; }
    public int Stateid { get; set; }
    public int Cityid { get; set; }
}
