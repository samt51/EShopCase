namespace EShopCase.Application.Filters;

[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class SwaggerDescriptionAttirbute : Attribute
{
    public string Summary { get; }
  

    public SwaggerDescriptionAttirbute(string summary)
    {
        Summary = summary;
    }
}