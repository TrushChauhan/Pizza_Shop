namespace backend.Models;

public class ChangePassword
{
    public string CurrentPassword { get; set; }

    public string NewPassword { get; set; }
    
    public string ConfirmNewPassword { get; set; }
}
