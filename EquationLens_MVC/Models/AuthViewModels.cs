using System.ComponentModel.DataAnnotations;
namespace EquationLens.Models;
public class RegisterViewModel {
 [Required, StringLength(100)] [Display(Name="Full name")] public string FullName {get;set;}="";
 [Required, EmailAddress] public string Email {get;set;}="";
 [Required, MinLength(8)] [DataType(DataType.Password)] public string Password {get;set;}="";
 [Required, DataType(DataType.Password)] [Compare(nameof(Password),ErrorMessage="Passwords do not match.")] [Display(Name="Confirm password")] public string ConfirmPassword {get;set;}="";
}
public class LoginViewModel {
 [Required,EmailAddress] public string Email {get;set;}="";
 [Required,DataType(DataType.Password)] public string Password {get;set;}="";
 [Display(Name="Keep me logged in")] public bool RememberMe {get;set;}
}
