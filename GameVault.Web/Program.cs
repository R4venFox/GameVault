using GameVault.Business;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews(options =>
{
    options.ModelBindingMessageProvider.SetAttemptedValueIsInvalidAccessor(
        (value, field) => $"Vrednost za polje {field} nije ispravna.");
    options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
        _ => "Unesite vrednost.");
    options.ModelBindingMessageProvider.SetValueIsInvalidAccessor(
        _ => "Uneta vrednost nije ispravna.");
});
builder.Services.AddBusiness(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler(errorApp => errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await context.Response.WriteAsync("Doslo je do greske pri obradi zahteva.");
    }));
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePages("text/plain; charset=utf-8", "Zahtev nije moguće obraditi. HTTP status: {0}.");
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
