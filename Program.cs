using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Python.Runtime;
using StoreAnalysis.Data;
using StoreAnalysis.Script;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("StoreAnalysisContextConnection") ?? throw new InvalidOperationException("Connection string 'StoreAnalysisContextConnection' not found.");

Runtime.PythonDLL = @"C:\Users\AZPC\AppData\Local\Programs\Python\Python310\python310.dll";
PythonEngine.Initialize();
PythonEngine.BeginAllowThreads();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<StoreAnalysisContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<StoreAnalysisContext>();

builder.Services.AddSingleton<TelegramService>(provider =>
    new TelegramService(
        provider.GetRequiredService<IHttpClientFactory>().CreateClient(),
        "7911455670:AAF1ot2IqIsAbfF7urdeTo4_UIocXmKLZfQ" // Thay bằng token mới từ BotFather
        , "-4630519621"
    )
);
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();// nếu không có dòng này thì login thành công vẫn không hiển thị được trạng thái đã đăng nhập
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.Run();
