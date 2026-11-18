namespace EquationLens.Models;
public class WorkspaceViewModel { public string FullName {get;set;}=""; public List<EquationRecord> History {get;set;}=new(); public string? Equation {get;set;} public List<string>? Steps {get;set;} public string? Error {get;set;} }
public class AdminDashboardViewModel { public int UserCount {get;set;} public int EquationCount {get;set;} public List<UserSummary> Users {get;set;}=new(); }
public class UserSummary { public string Name {get;set;}=""; public string Email {get;set;}=""; public string Role {get;set;}="User"; public int Solved {get;set;} }
