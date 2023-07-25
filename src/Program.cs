using EmailSenderAPI.Models;
using EmailSenderAPI.Services.V1;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.LayoutRenderers;
using NLog.Web;
using AutoMapper;

//Early init of Nlog to allow startup and exception logging, before host in build
var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);
 
    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddMvc();

    //NLog: Setup Nlog for DI
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    //AutoMapper: Configure AutoMapper
    builder.Services.AddAutoMapper(opt=>
    {
        opt.AllowNullDestinationValues = true;
        opt.AddProfile(typeof(MailProfile));
    });
    
    //Configure Email Settings
    builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
    builder.Services.AddScoped<IEmailProvider, EmailProvider>();
    builder.Services.Configure<MailData>(builder.Configuration);

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "Email Sender Api",
            Version = "v1",
            Description = "Sending Email with Mimekit and Mailkit smtp",
        });
    });
    // Install-Package Microsoft.AspNetCore.Mvc.Versioning
    builder.Services.AddApiVersioning(opt =>
    {
        opt.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1,0);
        opt.AssumeDefaultVersionWhenUnspecified = true;
        opt.ReportApiVersions = true;
        opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                                                        new HeaderApiVersionReader("x-api-version"),
                                                        new MediaTypeApiVersionReader("x-api-version"));


    });

    //Cors Configuration 
    builder.Services.AddCors(opts =>
    {
        opts.AddPolicy("CorsPolicy", builder =>
        {
            builder.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
        });
    });

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Email Sender by Gmail");
        });
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch(Exception ex)
{
    logger.Error(ex, "Stopped Program because of exception");
}
finally
{
    //Ensure to flush and stop internal timers/threads before application-exit (Avoid segmantation fault on Linux)
    NLog.LogManager.Shutdown();
}



