using ERMS.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    var timeout = builder.Configuration.GetValue<int>("Session:TimeoutMinutes");
    options.IdleTimeout = TimeSpan.FromMinutes(timeout > 0 ? timeout : 60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = builder.Configuration.GetValue<string>("Session:CookieName") ?? "ERMS.Session";
});

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// HttpClient for API calls
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl") ?? "https://localhost:7001";
builder.Services.AddHttpClient("ERMSAPI", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
}).ConfigurePrimaryHttpMessageHandler(() =>
{
    return new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    };
});

// API Service
builder.Services.AddScoped<IApiService, ApiService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
