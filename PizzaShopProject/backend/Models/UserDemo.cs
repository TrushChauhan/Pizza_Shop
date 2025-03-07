using System.ComponentModel.DataAnnotations;

namespace backend.Models;
public class UserDemo
{
    public string Email { get; set; }

    public string Password{ get; set; }
    public bool IsRemember{get; set;}
}
