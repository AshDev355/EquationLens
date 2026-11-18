using System.Globalization;
using System.Text.RegularExpressions;
namespace EquationLens.Services;
public record SolveResult(List<string> Steps, string Answer);
public static class EquationSolver {
 public static SolveResult Solve(string input) {
  if (string.IsNullOrWhiteSpace(input)) throw new ArgumentException("Enter an equation first.");
  var s=input.Replace(" ","").Replace("−","-").Replace("×","*").Replace("\\cdot","*").Replace("*","");
  var sides=s.Split('='); if(sides.Length!=2) throw new ArgumentException("Enter one equation using =, for example: 2x^2 - 5x + 3 = 0.");
  var (a1,b1,c1)=Parse(sides[0]); var (a2,b2,c2)=Parse(sides[1]); double a=a1-a2,b=b1-b2,c=c1-c2;
  var steps=new List<string>();
  if(Math.Abs(a)<1e-10 && Math.Abs(b)<1e-10) { var ans=Math.Abs(c)<1e-10?"Every real number is a solution.":"No solution."; steps.Add(ans); return new(steps,ans); }
  if(Math.Abs(a)<1e-10) { steps.Add($"Rearrange: {F(b)}x = {F(-c)}"); var x=-c/b; steps.Add($"x = {F(-c)} / {F(b)}"); var answer=$"x = {F(x)}"; steps.Add(answer); return new(steps,answer); }
  var d=b*b-4*a*c; steps.Add($"a = {F(a)}, b = {F(b)}, c = {F(c)}"); steps.Add($"Discriminant: Δ = b² − 4ac = {F(d)}");
  if(d < -1e-10) { var answer="No real roots."; steps.Add(answer); return new(steps,answer); }
  if(Math.Abs(d)<1e-10) { var x=-b/(2*a); var answer=$"x = {F(x)} (repeated root)"; steps.Add($"x = −b / 2a = {F(x)}"); steps.Add(answer); return new(steps,answer); }
  var root=Math.Sqrt(d); steps.Add($"x = (−b ± √Δ) / 2a = ({F(-b)} ± {F(root)}) / {F(2*a)}"); var x1=(-b-root)/(2*a); var x2=(-b+root)/(2*a); var result=$"x = {F(Math.Min(x1,x2))} or x = {F(Math.Max(x1,x2))}"; steps.Add(result); return new(steps,result);
 }
 static (double a,double b,double c) Parse(string side) {
  side=Regex.Replace(side,@"(?<=[0-9x])(?=x)","*"); side=side.Replace("-","+-"); if(side.StartsWith('+')) side=side[1..]; double a=0,b=0,c=0;
  foreach(var term in side.Split('+',StringSplitOptions.RemoveEmptyEntries)) {
   var t=term.Replace("*",""); var m=Regex.Match(t,@"^([+-]?\d*\.?\d*)(x(?:\^2)?)?$"); if(!m.Success) throw new ArgumentException("This demo solver supports linear and quadratic equations in x. Check the equation format.");
   var n=m.Groups[1].Value; double coef= n==""||n=="+"?1:n=="-"?-1:double.Parse(n,CultureInfo.InvariantCulture);
   if(m.Groups[2].Value=="x^2") a+=coef; else if(m.Groups[2].Value=="x") b+=coef; else c+=coef;
  } return(a,b,c);
 }
 static string F(double n) { if(Math.Abs(n)<1e-10)n=0; return Math.Round(n,6).ToString("0.######",CultureInfo.InvariantCulture); }
}
